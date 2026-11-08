#!/usr/bin/env bash
# Consumer smoke test (RIZZY-001).
#
# Installs the freshly packed Rizzy packages into a brand-new app, using PACKAGE references
# (not project references), restores from an isolated package cache and renders a Rizzy
# component. This catches mismatched Rizzy / Rizzy.Htmx versions that a solution build
# cannot see.
#
# Usage: eng/consumer-smoke-test.sh <directory-containing-nupkgs> <version>
set -euo pipefail

# pwd -W gives a native Windows path under Git Bash (needed inside nuget.config).
PKG_DIR="$(cd "$1" && (pwd -W 2>/dev/null || pwd))"
VERSION="$2"
WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT

echo "Smoke-testing Rizzy $VERSION from $PKG_DIR in $WORK"

mkdir -p "$WORK/app"
cat > "$WORK/nuget.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$PKG_DIR" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="Rizzy" />
      <package pattern="Rizzy.*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
EOF

cat > "$WORK/app/SmokeApp.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Rizzy" Version="$VERSION" />
  </ItemGroup>
</Project>
EOF

cat > "$WORK/app/Program.cs" <<'EOF'
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy;
using Rizzy.Htmx;

// Same registrations as a real Rizzy app.
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddRazorComponents();
builder.Services.AddRizzy();
builder.Services.AddHtmx();

await using var app = builder.Build();
await using var scope = app.Services.CreateAsyncScope();
await using var renderer = new HtmlRenderer(scope.ServiceProvider, scope.ServiceProvider.GetRequiredService<ILoggerFactory>());

// Rendering HtmxSwappable exercises Rizzy -> Rizzy.Htmx APIs that were missing from the
// mismatched packages published for 5.4.0 (HtmxAttributeName, HtmxConfig.Prefix).
var html = await renderer.Dispatcher.InvokeAsync(async () =>
{
    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
    {
        ["TargetId"] = "target",
        ["ChildContent"] = (RenderFragment)(b => b.AddContent(0, "hello")),
    });
    var output = await renderer.RenderComponentAsync<HtmxSwappable>(parameters);
    return output.ToHtmlString();
});

var config = new HtmxConfig { History = HtmxHistoryMode.Reload }.Serialize();

Console.WriteLine(html);
Console.WriteLine(config);

if (!html.Contains("<hx-partial") || !config.Contains("\"history\":\"reload\""))
{
    Console.Error.WriteLine("Unexpected output from the installed Rizzy packages.");
    return 1;
}

return 0;
EOF

cd "$WORK/app"
dotnet restore --configfile "$WORK/nuget.config" --packages "$WORK/packages"

# Both packages must resolve to exactly the version under test.
PYTHON="$(command -v python3 || command -v python)"
"$PYTHON" - "$WORK/app/obj/project.assets.json" "$VERSION" <<'PY'
import json, sys
assets = json.load(open(sys.argv[1], encoding="utf-8"))
expected = sys.argv[2]
resolved = {key.split("/")[0]: key.split("/")[1] for key in assets["libraries"]}
for package in ("Rizzy", "Rizzy.Htmx"):
    actual = resolved.get(package)
    print(f"{package} resolved to {actual}")
    if actual != expected:
        sys.exit(f"{package} resolved to {actual}, expected {expected}")
PY

dotnet run --no-restore -c Release
echo "Consumer smoke test passed for Rizzy $VERSION"
