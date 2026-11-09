#!/usr/bin/env python3
"""Validate the contents of the Rizzy NuGet packages (RIZZY-001, RIZZY-021).

Usage:
    python eng/check-package-contents.py <directory-containing-nupkgs> [expected-version]

Checks:
  * Rizzy and Rizzy.Htmx packages are both present and have the same version
    (lockstep versioning).
  * Rizzy depends on exactly that version of Rizzy.Htmx.
  * Every file in each package is on an allow-list, so stray tooling files,
    publish output or stale bundles can never ship again.
  * Required files (assembly, XML docs, client bundle, source generator) are present.
"""

import fnmatch
import re
import sys
import zipfile
from pathlib import Path

COMMON_ALLOWED = [
    "_rels/.rels",
    "[[]Content_Types].xml",  # brackets escaped for fnmatch
    "package/services/metadata/core-properties/*.psmdcp",
    "*.nuspec",
    "LICENSE.md",
    "README.md",
    "rizzy.png",
]

PACKAGES = {
    "Rizzy": {
        "allowed": COMMON_ALLOWED + [
            "lib/net10.0/Rizzy.dll",
            "lib/net10.0/Rizzy.xml",
            "build/*.props",
            "buildMultiTargeting/*.props",
            "buildTransitive/*.props",
            "staticwebassets/js/rizzy.js",
            "staticwebassets/js/rizzy.js.map",
            "staticwebassets/js/rizzy.min.js",
            "analyzers/dotnet/cs/Rizzy.SourceGenerators.dll",
        ],
        "required": [
            "lib/net10.0/Rizzy.dll",
            "lib/net10.0/Rizzy.xml",
            "staticwebassets/js/rizzy.js",
            "staticwebassets/js/rizzy.min.js",
            "analyzers/dotnet/cs/Rizzy.SourceGenerators.dll",
        ],
    },
    "Rizzy.Htmx": {
        "allowed": COMMON_ALLOWED + [
            "lib/net10.0/Rizzy.Htmx.dll",
            "lib/net10.0/Rizzy.Htmx.xml",
        ],
        "required": [
            "lib/net10.0/Rizzy.Htmx.dll",
            "lib/net10.0/Rizzy.Htmx.xml",
        ],
    },
}

PACKAGE_FILE = re.compile(r"^(?P<id>Rizzy(?:\.Htmx)?)\.(?P<version>\d+\.\d+\.\d+(?:-[0-9A-Za-z.\-]+)?)\.nupkg$")


def fail(errors: list[str], message: str) -> None:
    errors.append(message)
    print(f"ERROR: {message}")


def main() -> int:
    if len(sys.argv) < 2:
        print(__doc__)
        return 2

    directory = Path(sys.argv[1])
    expected_version = sys.argv[2] if len(sys.argv) > 2 else None
    errors: list[str] = []

    found: dict[str, tuple[str, Path]] = {}
    for path in sorted(directory.glob("*.nupkg")):
        match = PACKAGE_FILE.match(path.name)
        if match:
            found[match["id"]] = (match["version"], path)

    for package_id in PACKAGES:
        if package_id not in found:
            fail(errors, f"package {package_id} not found in {directory}")

    if errors:
        return 1

    versions = {package_id: version for package_id, (version, _) in found.items()}
    if len(set(versions.values())) != 1:
        fail(errors, f"packages are not versioned in lockstep: {versions}")

    version = versions["Rizzy"]
    if expected_version and version != expected_version:
        fail(errors, f"expected version {expected_version}, found {version}")

    for package_id, rules in PACKAGES.items():
        _, path = found[package_id]
        with zipfile.ZipFile(path) as archive:
            names = [name for name in archive.namelist() if not name.endswith("/")]

            for name in names:
                if not any(fnmatch.fnmatchcase(name, pattern) for pattern in rules["allowed"]):
                    fail(errors, f"{path.name}: unexpected file '{name}'")

            for required in rules["required"]:
                if required not in names:
                    fail(errors, f"{path.name}: missing required file '{required}'")

            if package_id == "Rizzy":
                nuspec_name = next(n for n in names if n.endswith(".nuspec"))
                nuspec = archive.read(nuspec_name).decode("utf-8-sig")
                dependency = re.search(r'<dependency id="Rizzy\.Htmx" version="([^"]+)"', nuspec)
                if not dependency:
                    fail(errors, "Rizzy.nuspec has no dependency on Rizzy.Htmx")
                elif dependency.group(1).strip("[]()") != version:
                    fail(errors, f"Rizzy depends on Rizzy.Htmx {dependency.group(1)}, expected {version}")

    if errors:
        print(f"\n{len(errors)} package problem(s) found.")
        return 1

    print(f"Package contents OK: Rizzy and Rizzy.Htmx {version}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
