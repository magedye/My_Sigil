# Upgrade Notes

## Breaking Changes
- The library targets **.NET 8** exclusively. Previous `net461` and .NET Standard targets have been removed.
- C# language version is now `12.0`.

## New Features
- Disassembler now resolves members using generic type context, improving support for generic methods.
- Cast transitions for `Emit.CastClass` use `WildcardType` for more accurate stack behavior.

## Fixes
- Corrected `ResolveMember` invocation when disassembling generic code.
- Fixed readonly patch index adjustment when emitting volatile operations.
- Pointer tests free allocated memory using `try/finally` to avoid leaks.
- Internal helper renamed to `IsLegalConstructorCall`.

## Dependencies
- Build requires the **.NET 8 SDK** (`8.0.116`).
- Tooling packages updated to `Nerdbank.GitVersioning` **3.7.115** and `Microsoft.SourceLink.GitHub` **8.0.0**.
- Test packages updated to `Microsoft.NET.Test.Sdk` **17.14.1** and `xUnit` **2.9.3**.
