# netstandard2.1 sidecar build

The main project cannot target netstandard2.1 directly: Roslyn error CS8919 forbids `static abstract` interface members for that target (the `TPolicy.Fail(...)` dispatch needs a .NET 7+ runtime). The `ValidSphere.Ns21` sidecar project works around this by compiling generated copies of the main sources with instance dispatch instead. Its DLL ships in the same NuGet package under `lib/netstandard2.1/`.

## Layout

- `src/ValidSphere/` — the only hand-written sources. Single source of truth.
- `src/ValidSphere.Ns21/` — project file, generator task, two hand-written files, and `gen/` (generated, git-ignored).
- `src/ValidSphere.Ns21/gen/` — regenerated on every build, never edited.

## Generation (`BeforeBuild`)

The inline MSBuild task `GenerateNs21Sources` (RoslynCodeTaskFactory, no shell dependency) wipes `gen/` and copies every `src/ValidSphere/*.cs` except:

- `ComparisonAssertions.cs`, `NumericAssertions.cs`, `FloatingPointAssertions.cs` — generic-math interfaces (`System.Numerics`) do not exist on netstandard2.1.
- `DateOnlyAssertions.cs` — `DateOnly` (.NET 6+) does not exist on netstandard2.1.
- `AssertionPolicy.cs` — replaced by the hand-written instance variant.

Three mechanical rewrites apply to the copies:

- `TPolicy.Fail(|FailNull(|FailOutOfRange(` → `default(TPolicy).…(`
- `IsPolicy.…(` → `default(IsPolicy).…(` (entry points in `Assertion.cs`, `StringEntryExtensions.cs`)
- `GuardPolicy.…(` → `default(GuardPolicy).…(`

Constrained calls on the policy struct stay allocation-free, like the static variant.

## Hand-written files

- `Policy/AssertionPolicy.cs` — `IAssertionPolicy` with the same three members as instance methods, `IsPolicy`/`GuardPolicy` with identical method bodies and exception types, plus the same attributes (`DoesNotReturn` exists on netstandard2.1).
- `CompilerPolyfills.cs` — `CallerArgumentExpressionAttribute` and `StackTraceHiddenAttribute` (both missing on netstandard2.1).

Smaller BCL gaps live in the shared sources so both legs stay in sync: `ThrowHelper` (replaces `ThrowIfNull`), a try/catch branch for `MailAddress.TryCreate`, a delete-then-move branch for the 3-argument `File.Move`, and a shared `TrimTrailingSeparator` helper.

## Packaging

The main project builds the sidecar on every build (`ProjectReference` with `ReferenceOutputAssembly="false"` — build coupling without a compile reference, since both assemblies define the same types). At pack time the `AddNs21ToPackage` target (before `_GetPackageFiles`) rebuilds the sidecar and registers its DLL as `lib/netstandard2.1/`. The sidecar itself is `IsPackable=false`, so only one nupkg is produced. `NU5128` is suppressed with a comment: NuGet cannot infer the (empty) dependency group for the manually contributed asset.

## Resulting API surface

The ns2.1 leg contains everything except the four excluded files: no generic-math comparisons (`Greater`, `Less`, `Range`, …), no numeric (`Positive`, `Even`, …), no floating-point (`Approx`, `Finite`, …), and no date-only (`Today`, `Weekday`) checks. It runs on any runtime that loads netstandard2.1, including old ones (.NET Core 3.1/5/6), because instance dispatch needs no modern JIT.

## Local commands (repo root)

```powershell
dotnet build ValidSphere.slnx -c Release            # builds everything incl. generation, 0/0 expected
dotnet pack src/ValidSphere/ValidSphere.csproj -c Release --output artifacts
```

Verify the package contains `lib/net10.0/`, `lib/net8.0/`, and `lib/netstandard2.1/ValidSphere.dll`, and that `gen/` holds transformed sources (no `TPolicy.Fail(` without a `default(` prefix).
