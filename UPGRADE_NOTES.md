# Upgrade Notes

The 5.0.0 release modernizes Sigil and its build.

## Changes since 4.7.0
- Switched to the SDK style project system with git-based versioning and SourceLink.
- Updated target to `net8.0`.
- Converted tests to xUnit and run via `dotnet test`.
- Updated IL and disassembler tests and removed standalone runners.
