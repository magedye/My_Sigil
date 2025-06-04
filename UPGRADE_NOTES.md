# Upgrade Notes

The project has been migrated to .NET 8.

## Project changes
- Global SDK version updated to `8.0.116` via `global.json`.
- `Sigil` library now targets `net8.0` only and uses the built‑in `System.Reflection.Emit` APIs.
- Tests target `net8.0` and use the latest xUnit and `Microsoft.NET.Test.Sdk` packages.
- Build tooling updated to the latest `Nerdbank.GitVersioning` and `Microsoft.SourceLink.GitHub` packages.
- C# language version set to `12.0`.

The solution builds with Visual Studio 2022 or the .NET 8 SDK.
