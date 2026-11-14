#!/usr/bin/env bash
# Prints the CHANGELOG.md section for a version (without its heading), for the GitHub release notes.
# Fails when the changelog has no "## [<version>]" heading, so a release cannot go out without one.
#
# Usage: eng/changelog-section.sh <version>
set -euo pipefail
version="${1:?usage: eng/changelog-section.sh <version>}"
file="$(dirname "$0")/../CHANGELOG.md"

section="$(awk -v heading="## [${version}]" '
    index($0, heading) == 1 { found = 1; next }
    found && /^## / { exit }
    found && /^\[[^]]+\]: / { exit }
    found { print }
' "$file")"

if [[ -z "${section//[[:space:]]/}" ]]; then
    echo "CHANGELOG.md has no section for ${version} (expected a \"## [${version}]\" heading)." >&2
    exit 1
fi
printf '%s\n' "$section"
