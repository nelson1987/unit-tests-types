#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "[pre-push] Running local validation checks..."

# Restore and build
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet restore UnitTestsTypes.sln
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build UnitTestsTypes.sln --configuration Release --no-restore

# Unit tests
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test tests/UnitTestsTypes.UnitTests/UnitTestsTypes.UnitTests.csproj --configuration Release --no-build --verbosity minimal

# Integration tests
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test tests/UnitTestsTypes.IntegrationTests/UnitTestsTypes.IntegrationTests.csproj --configuration Release --no-build --verbosity minimal

# Format check
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet format UnitTestsTypes.sln --verify-no-changes

# Optional lightweight check for tools that are installed
if command -v k6 >/dev/null 2>&1; then
  echo "[pre-push] k6 is available"
else
  echo "[pre-push] k6 is not installed; skipping performance check"
fi

echo "[pre-push] Validation completed successfully"
