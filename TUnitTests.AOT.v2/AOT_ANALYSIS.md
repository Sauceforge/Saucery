# AOT analysis

## What changed

`REST/RESTTests.cs` was rewritten to remove runtime reflection entirely.

The previous version used:
- `GetMethods(...)`
- `MakeGenericMethod(...)`
- `MethodInfo.Invoke(...)`
- assembly scanning via `AppDomain.CurrentDomain.GetAssemblies()`

Those patterns are fragile under trimming and Native AOT because they depend on runtime metadata being preserved.

The rewritten version now uses:
- direct compile-time calls to `GetPlatform<TPlatform>()`
- TUnit generic test generation via repeated `[GenerateGenericTest(typeof(...))]`
- strongly typed assertions on `List<TPlatform>`

## Why this is more AOT-friendly

TUnit's AOT guidance for generic tests is to instantiate generic tests explicitly with `[GenerateGenericTest(...)]`, so the test cases are source-generated at compile time rather than discovered through runtime reflection.

This rewrite aligns with that guidance and removes the reflection-heavy helper from the test surface.

## Remaining caveat

This test project is now substantially more Native-AOT-friendly from the test-code side.

However, whether the project is *fully* Native AOT compliant still depends on `Saucery.Core` itself:
- `GetPlatform<T>()` must be available to the compiler as a normal strongly typed API
- any implementation inside `Saucery.Core` must also avoid trim/AOT-unsafe reflection unless properly annotated or preserved

So the right skeptical framing is:

- `REST/RESTTests.cs` no longer contains the reflection caveat
- the final proof still comes from a successful `dotnet publish -c Release` Native AOT publish of the whole solution
