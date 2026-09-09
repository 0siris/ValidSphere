# ValidSphere

[![NuGet](https://img.shields.io/nuget/v/ValidSphere.svg)](https://www.nuget.org/packages/ValidSphere)
[![CI](https://github.com/0siris/ValidSphere/actions/workflows/ci.yml/badge.svg)](https://github.com/0siris/ValidSphere/actions/workflows/ci.yml)
[![Deterministic](https://img.shields.io/badge/build-deterministic-brightgreen.svg)](https://learn.microsoft.com/nuget/create-packages/deterministic-packages)
[![SourceLink](https://img.shields.io/badge/Source%20Link-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

A lightweight, extensible assertion and guard library for modern .NET.

`ValidSphere` constrains program execution to valid states by rejecting invalid inputs and runtime conditions where their constraints are known. It is runtime-first and intended for production code as well as tests, but it is not primarily a test assertion framework.

The same assertion extensions can be used with different **failure policies**:

```csharp
value.Is().Eq(expected);
value.Guard().Eq(expected);
```

The assertion logic stays the same. Only the failure behavior changes:

- `Is()` represents a runtime assertion and throws `AssertException`.
- `Guard()` represents argument or precondition validation and throws standard .NET argument exceptions.

This keeps validation rules reusable while preserving the correct failure semantics for each call site.

## Contents

- [Positioning](#positioning)
- [Goals](#goals)
- [Non-Goals](#non-goals)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Basic Usage](#basic-usage)
  - [Guards](#guards)
  - [`Is()` vs. `Guard()`](#is-vs-guard)
  - [Chaining Assertions](#chaining-assertions)
  - [Using the Asserted Value](#using-the-asserted-value)
- [Assertion Reference](#assertion-reference)
  - [Common Assertions](#common-assertions)
  - [Nullability](#nullability)
  - [Numeric Comparisons](#numeric-comparisons)
  - [Floating-Point Assertions](#floating-point-assertions)
  - [Strings](#strings)
  - [GUIDs](#guids)
  - [Collections](#collections)
  - [Paths, URIs, and Mail Addresses](#paths-uris-and-mail-addresses)
  - [Runtime Type Assertions](#runtime-type-assertions)
  - [Optional Failure Messages](#optional-failure-messages)
- [Diagnostics and Testing](#diagnostics-and-testing)
  - [Caller Information and Debugging](#caller-information-and-debugging)
  - [Test Framework Integration](#test-framework-integration)
- [Architecture and Extending](#architecture-and-extending)
  - [Architecture](#architecture)
  - [`Assertion<T, TPolicy>`](#assertiont-tpolicy)
  - [`AssertionContext`](#assertioncontext)
  - [Assertion Policies](#assertion-policies)
  - [Implementing a Custom Policy](#implementing-a-custom-policy)
  - [Creating an Entry Point for a Custom Policy](#creating-an-entry-point-for-a-custom-policy)
  - [Writing Custom Assertion Extensions](#writing-custom-assertion-extensions)
  - [Preserving Assertion Context in Extensions](#preserving-assertion-context-in-extensions)
  - [Example: Domain-Specific Extension](#example-domain-specific-extension)
  - [Example: Refining the Assertion Type](#example-refining-the-assertion-type)
  - [Extension Design Guidelines](#extension-design-guidelines)
- [Appendix](#appendix)
  - [Performance Model](#performance-model)
  - [Choosing Between Specialized Assertions and `Satisfy()`](#choosing-between-specialized-assertions-and-satisfy)
  - [Recommended Usage](#recommended-usage)
  - [API Overview](#api-overview)
  - [Summary](#summary)

---

## Positioning

ValidSphere sits between classic guard libraries and test assertion libraries:

| Guard libraries | ValidSphere | Test assertions |
| --- | --- | --- |
| Guard clauses | Guards and invariants | Test-focused assertions |
| Caller contracts | Runtime-first | Test diagnostics |
| Precondition checks | Reusable rules | Expected-result checks |

The name refers to the valid runtime state space that remains after guards, assertions, and invariants eliminate invalid states.

---

## Goals

The library is designed around a few principles:

- **Small API surface**
- **Fluent but lightweight syntax**
- **No heap allocation introduced by the core assertion wrapper on the successful path**
- **No reflection**
- **No stack walking for caller information**
- **No virtual dispatch for assertion policies**
- **Reusable assertion extensions**
- **Separate failure semantics through policies**
- **Good nullable-flow integration**
- **Good compiler and debugger integration**
- **Caller expression, member, file, and line captured automatically**
- **Assertions can be embedded directly into expressions**
- **Easy to extend with project-specific assertions and policies**

The core assertion object is a `readonly struct`, while policies use static abstract interface members.

```text
value
  │
  ├── Is()
  │     └── IsPolicy
  │            └── AssertException
  │
  └── Guard()
        └── GuardPolicy
               ├── ArgumentException
               ├── ArgumentNullException
               └── ArgumentOutOfRangeException
```

---

## Non-Goals

ValidSphere is not a DTO or object validation framework, a replacement for FluentValidation, a full test assertion or object-graph comparison library, or a reflection-based diagnostics and contract system.

---

# Getting Started

## Installation

Add the project or package reference to the consuming project.
The package targets `net10.0`, `net8.0` (identical API) and `netstandard2.1` (without generic-math comparisons, numeric, floating-point, and date-only checks).

Builds are deterministic (`Deterministic`, `ContinuousIntegrationBuild` on CI) and SourceLink-enabled (commit in the product version, PDBs in the symbol package). Verify with `dotnet tool install -g sourcelink` followed by `sourcelink test <package>.pdb`.

For the preview package:
```xml
<PackageReference Include="ValidSphere" Version="0.2.0-preview.4" />
```

For a source/project reference:

```xml
<ItemGroup>
    <ProjectReference Include="..\extern\ValidSphere\ValidSphere.csproj" />
</ItemGroup>
```

Then import the namespace:

```csharp
using ValidSphere;
```

---

## Basic Usage

### Runtime assertions with `Is()`

Use `Is()` when a failed condition represents an invalid program state, violated invariant, unexpected result, or failed test assertion.

```csharp
var count = 5;

count.Is().Eq(5);
count.Is().Greater(0);
```

A failed `Is()` assertion throws `AssertException`.

```csharp
var count = 3;

count.Is().Eq(5);
```

A failure contains the assertion message together with compiler-captured source information, for example:

```text
Expected '5', but found '3'.
Expression: count
Member: Process
Source: C:\Projects\Example\Service.cs(42)
```

The original source expression is captured with `CallerArgumentExpression`.

This also works for expressions:

```csharp
items.Count.Is().Greater(0);
```

The assertion context can therefore contain:

```text
Expression: items.Count
```

without reflection, stack-trace parsing, or source-code analysis.

---

## Guards

Use `Guard()` for method arguments and preconditions.

```csharp
public void SetRetryCount(int retryCount)
{
    retryCount.Guard().Range(0, 10);

    // ...
}
```

The important difference from `Is()` is the exception type.

For example:

```csharp
name.Guard().NotNull();
```

throws `ArgumentNullException`, while:

```csharp
count.Guard().Greater(0);
```

throws `ArgumentOutOfRangeException`.

Other guard failures use `ArgumentException`.

The captured subject expression is used as the parameter name where possible, so:

```csharp
retryCount.Guard().Greater(0);
```

can produce an exception whose `ParamName` is:

```text
retryCount
```

This allows public APIs to use the fluent assertion syntax while still following normal .NET argument-exception conventions.

If a nullable variable should also be recognized as non-null by C# nullable flow analysis after the call, use `AsGuardNotNull()` as described below.

---

## `Is()` vs. `Guard()`

Both entry points use the same assertion extensions.

```csharp
value.Is().Greater(0);
value.Guard().Greater(0);
```

The difference is the selected policy.

| Entry point | Intended use | Failure |
| --- | --- | --- |
| `Is()` | Runtime assertions, invariants, test assertions | `AssertException` |
| `Guard()` | Arguments and preconditions | Standard .NET argument exceptions |

For example:

```csharp
public Mesh Process(Mesh? mesh, float tolerance)
{
    mesh.AsGuardNotNull();
    tolerance.Guard().Greater(0);

    var result = ProcessInternal(mesh, tolerance);

    result.AsNotNull();

    return result;
}
```

`Guard()` and `AsGuardNotNull()` validate the caller contract.

`Is()` and `AsNotNull()` validate assumptions and invariants made by the implementation.

---

## Chaining Assertions

Assertion methods return the assertion object again, so checks can be chained directly.

```csharp
value.Is()
     .Greater(0)
     .Less(100)
     .NotEq(42);
```

For strings:

```csharp
name.Is()
    .NotNullOrWhiteSpace()
    .NotEmpty();
```

The selected policy and the original `AssertionContext` are retained for the complete chain.

Conceptually:

```text
value.Is()
    │
    └── Assertion<int, IsPolicy>
             │
             ├── Greater(0)
             ├── Less(100)
             └── NotEq(42)
```

No `And` property is required because every assertion directly returns an assertion that can be continued.

---

## Using the Asserted Value

`Assertion<T, TPolicy>` exposes the asserted value through `Value` and can also be converted implicitly back to `T`.

This allows assertions to be embedded directly into expressions:

```csharp
var count = input.Guard().Greater(0).Value;
```

or:

```csharp
Process(input.Guard().Greater(0));
```

Type-refining assertions can also be consumed inline:

```csharp
string? name = GetName();

Process(name.Guard().NotNull());
```

When the original nullable variable is used again afterwards, prefer the flow-analysis-aware direct entry point:

```csharp
string? name = GetName();

name.AsGuardNotNull();

Process(name); // no nullable warning
```

The two forms therefore solve slightly different problems:

```text
Guard().NotNull()
    → refines the returned Assertion<T, TPolicy>

AsGuardNotNull()
    → returns the validated non-null value directly
    → also informs nullable flow analysis about the original variable
```

The same principle applies to `AsNotNull()`.

---

# Assertion Reference

## Common Assertions

### Equality

Use `Eq()` and `NotEq()` for value equality:

```csharp
value.Is().Eq(expected);
value.Is().NotEq(unexpected);
```

Equality uses:

```csharp
EqualityComparer<T>.Default
```

The same extensions work as guards:

```csharp
mode.Guard().NotEq(Mode.Invalid);
```

### Boolean values

Boolean assertions use the shortened `True()` and `False()` members:

```csharp
isValid.Is().True();
hasErrors.Is().False();
```

They also work with `Guard()`:

```csharp
isSupported.Guard().True("The requested operation is not supported.");
```

### Arbitrary conditions

`Satisfy()` has two overloads.

#### Already evaluated condition

Use the Boolean overload for an arbitrary condition that is already available at the call site:

```csharp
facet.Is().Satisfy(
    facet.loops.Count == 0 ||
    facet.loops[^1] <= facet.outputVertexNos.Count);
```

The condition expression is captured by the compiler through `CallerArgumentExpression`, so a failure can report the original expression.

```text
Condition 'facet.loops.Count == 0 || facet.loops[^1] <= facet.outputVertexNos.Count' was not satisfied.
```

This overload does not require a delegate.

#### Predicate over the current subject

Use the predicate overload when the condition conceptually belongs to the current subject or should participate naturally in a fluent chain:

```csharp
facet.Is().Satisfy(
    static f => f.loops.Count == 0 ||
                f.loops[^1] <= f.outputVertexNos.Count);
```

This becomes especially useful after type or null refinement:

```csharp
string? text = GetText();

text.Is()
    .NotNull()
    .Satisfy(static value => value.Length >= 4);
```

After `NotNull()`, the predicate receives a non-null `string`.

For predicates that do not need external state, prefer a `static` lambda:

```csharp
value.Is().Satisfy(static x => x > 0);
```

A capturing lambda can allocate a closure:

```csharp
value.Is().Satisfy(x => x > minimum);
```

If the condition can be written directly without losing clarity, the Boolean overload avoids that delegate/closure concern:

```csharp
value.Is().Satisfy(value > minimum);
```

For frequently reused rules, prefer a dedicated assertion extension over `Satisfy()`.

---

## Nullability

### Fluent null assertions

Reference types can be checked with `NotNull()` and `Null()`:

```csharp
string? value = GetValue();

value.Is().NotNull();
value.Is().Null();
```

`NotNull()` refines:

```csharp
Assertion<string?, TPolicy>
```

to:

```csharp
Assertion<string, TPolicy>
```

so subsequent extensions operate on the non-null type:

```csharp
value.Is()
     .NotNull()
     .NotEmpty();
```

Nullable value types work the same way:

```csharp
int? value = GetValue();

value.Is()
     .NotNull()
     .Greater(0);
```

After `NotNull()`, the assertion contains an `int` rather than `int?`.

### Flow-analysis-aware null checks

In addition to the fluent null assertion, the library provides two direct null-check entry points:

```csharp
value.AsNotNull();
value.AsGuardNotNull();
```

Both exist for nullable reference types and nullable value types.

These direct methods additionally annotate the input with `[NotNull]`. This allows C# nullable flow analysis to understand that the original variable is non-null after a successful call.

For a reference type:

```csharp
Customer? customer = GetCustomer();

customer.AsGuardNotNull();

Handle(customer); // customer is known to be non-null
```

For a nullable value type:

```csharp
int? count = GetCount();

count.AsGuardNotNull();

var value = count.Value;
```

The direct methods differ only in failure semantics:

| Entry point | Intended use | Failure |
| --- | --- | --- |
| `AsNotNull()` | Runtime assertion / invariant | `AssertException` |
| `AsGuardNotNull()` | Argument / precondition | `ArgumentNullException` |

Both return the validated non-null value directly. To start a fluent chain instead, refine through `Is()`/`Guard()` first:

```csharp
string? name = GetName();

name.Guard().NotNullOrEmpty();
```

They can also be used directly inside expressions:

```csharp
Process(name.AsGuardNotNull());
```

Use the direct variants when the nullable state of the **original variable** matters after the call.

Use the fluent `Is().NotNull()` or `Guard().NotNull()` form when the refined assertion value is consumed directly or when the null check is naturally part of a chain.

---

## Numeric Comparisons

Comparison assertions use generic math operator interfaces.

```csharp
value.Is().Greater(10);
value.Is().GreaterEq(10);
value.Is().Less(100);
value.Is().LessEq(100);
```

Ranges are inclusive:

```csharp
value.Is().Range(0, 100);
```

Equivalent condition:

```text
0 <= value <= 100
```

The same assertions can be used as guards:

```csharp
percentage.Guard().Range(0, 100);
```

A failed comparison or range guard results in an `ArgumentOutOfRangeException`.

---

## Floating-Point Assertions

Floating-point values should often be compared using a tolerance rather than exact equality.

Use `Approx()`:

```csharp
double result = Calculate();

result.Is().Approx(
    expected: 10.0,
    tolerance: 0.0001);
```

The check is based on the absolute difference:

```text
|actual - expected| <= tolerance
```

Example:

```csharp
0.30000000000000004
    .Is()
    .Approx(0.3, 1e-12);
```

Negative or `NaN` tolerances are invalid and result in an `ArgumentOutOfRangeException`.

---

## Strings

Check for null or empty:

```csharp
string? name = GetName();

name.Is().NotNullOrEmpty();
```

Check for null, empty, or whitespace:

```csharp
name.Is().NotNullOrWhiteSpace();
```

After either check, the returned assertion contains a non-null `string`.

This allows further chaining:

```csharp
name.Is()
    .NotNullOrWhiteSpace()
    .NotEmpty();
```

For argument validation:

```csharp
public User(string? name)
{
    Name = name.Guard()
               .NotNullOrWhiteSpace();
}
```

If the original `name` variable must also be considered non-null afterwards, use `AsGuardNotNull()` first:

```csharp
name.Guard().NotNullOrEmpty();
```

---

## GUIDs

The current GUID-specific API uses `NotBeEmpty()`:

```csharp
id.Guard().NotBeEmpty();
```

For nullable GUIDs:

```csharp
Guid? id = GetId();

id.Is().NotBeNullOrEmpty();
```

The nullable form refines the assertion to:

```csharp
Assertion<Guid, TPolicy>
```

---

## Collections

Require at least one element:

```csharp
items.Is().NotEmpty();
```

Require an exact count:

```csharp
items.Is().HaveCount(3);
```

Arrays additionally support exact length checks:

```csharp
buffer.Is().HaveLength(1024);
```

The built-in collection assertions operate on `ICollection` or arrays and use `Count`/`Length` directly.

---

## Paths, URIs, Mail Addresses, and GUIDs

Strings parse into a domain value before checks run, so file semantics never leak onto a plain string.
`Is().X()` entries return the chainable assertion; `Is().AsX()` extracts the raw value terminally (`Guid g = sample.Is().AsGuid()`):

```csharp
trackingId.Is().Guid().NotEmpty();
path.Is().File().Exists().HaveExtension(".json");
url.Guard().Uri().Absolute().HaveScheme("https");
mail.Is().MailAddress().HaveHost("contoso.com");
```

`File()`/`Directory()` refine to `FilePath`/`DirectoryPath`, which also forward a curated set of members to `System.IO.File`/`Directory`, so asserting and working share one chain (`file.ReadAllText()`, `dir.GetFiles()`).

## Runtime Type Assertions

Runtime type checks can refine the assertion type through `OfType<T>()`.

```csharp
object value = GetValue();

value.Is().OfType<MyType>();
```

After the check, the returned assertion contains `MyType` rather than `object`.

This allows type-specific assertions without an explicit cast:

```csharp
value.Is()
     .OfType<MyType>()
     .Satisfy(static typed => typed.IsValid);
```

The original `AssertionContext` is preserved during the refinement.

---

## Optional Failure Messages

Assertions provide meaningful default failure messages, but each assertion also accepts an optional custom message.

Typical default messages include:

```text
Expected '10', but found '5'.
```

```text
Value must be greater than '0'.
```

```text
String must not be null.
```

```text
Expected count '3', but found '2'.
```

A custom message can provide domain-specific context:

```csharp
count.Is().Greater(
    0,
    "A mesh must contain at least one vertex.");
```

or for a guard:

```csharp
retryCount.Guard().Range(
    0,
    10,
    "Retry count is outside the supported range.");
```

Prefer messages that explain **why the invariant exists**, rather than merely restating the assertion.

Good:

```text
A mesh must contain at least one vertex.
```

Less useful:

```text
Count must be greater than zero.
```

The latter information is already represented by the assertion itself.

### Performance note for custom messages

A string literal does not require building a new message on every call:

```csharp
value.Is().Greater(0, "Value must be positive.");
```

However, a caller-side interpolated message is evaluated before the assertion method is entered:

```csharp
value.Is().Greater(
    0,
    $"Value {value} for item {itemId} must be positive.");
```

That message may therefore allocate even when the assertion succeeds.

The built-in default diagnostic strings are created only in the failure branch.

---

# Diagnostics and Testing

## Caller Information and Debugging

`Is()`, `Guard()`, `AsNotNull()`, and `AsGuardNotNull()` capture source information automatically.

The captured `AssertionContext` contains:

```text
Expression
MemberName
FilePath
LineNumber
```

A failure can therefore include diagnostics such as:

```text
Expression: mesh.Vertices.Count
Member: ProcessMesh
Source: C:\Projects\App\MeshProcessor.cs(142)
```

Caller information is provided by compiler attributes:

```csharp
CallerArgumentExpression
CallerMemberName
CallerFilePath
CallerLineNumber
```

This does not require reflection, stack walking, or source-code parsing.

The assertion implementation also uses debugger/runtime attributes such as:

```csharp
[DebuggerStepThrough]
[StackTraceHidden]
[DoesNotReturn]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
[MethodImpl(MethodImplOptions.NoInlining)]
```

The intended split is:

```text
successful / hot path
    → small assertion methods
    → aggressive inlining
    → debugger steps through infrastructure

failure / cold path
    → policy failure methods
    → no inlining
    → internal frames hidden from formatted stack traces
```

---

## Test Framework Integration

`AssertException` implements the marker interface:

```csharp
IAssertionException
```

The interface intentionally has no members.

This allows compatible test frameworks such as xUnit.net v3 to recognize `AssertException` as an assertion failure without requiring the core library to reference xUnit.

A test can therefore use the normal `Is()` entry point:

```csharp
[Fact]
public void Result_is_valid()
{
    var result = Calculate();

    result.Is().Eq(42);
}
```

A failed `Is()` check produces `AssertException`, while `Guard()` intentionally continues to produce normal .NET argument exceptions.

No xUnit-specific policy is required for the built-in `Is()` behavior.

A custom policy can still be created when another test framework or environment requires different failure semantics.

---

# Architecture and Extending

## Architecture

The core architecture consists of four small pieces:

```text
             AssertionContext
                    │
                    ▼
          Assertion<T, TPolicy>
                    │
          ┌─────────┴─────────┐
          │                   │
   Assertion extensions     TPolicy
          │                   │
 Eq / Greater / ...      failure behavior
                              │
                    ┌─────────┴──────────┐
                    │                    │
                 IsPolicy           GuardPolicy
```

The assertion extension contains the **condition**.

`AssertionContext` contains compiler-provided diagnostic information.

The policy contains the **failure behavior**.

For example, `Greater(0)` does not need to know whether it is being used as:

```csharp
value.Is().Greater(0);
```

or:

```csharp
value.Guard().Greater(0);
```

The extension only evaluates the rule and delegates failures to `TPolicy`.

---

## `Assertion<T, TPolicy>`

The assertion wrapper is a `readonly struct`.

It contains:

```text
subject
AssertionContext
```

and exposes:

```csharp
Value
Context
Expression
```

It also supports implicit conversion back to `T`:

```csharp
Process(value.Guard().Greater(0));
```

Its public constructor forms the minimal extension surface for type-refining custom assertions:

```csharp
new Assertion<TRefined, TPolicy>(
    refinedValue,
    assertion.Context);
```

No builder object or policy instance is required.

---

## `AssertionContext`

`AssertionContext` is a `readonly struct` containing the call-site information captured at the assertion entry point:

```csharp
public readonly struct AssertionContext
{
    public string? Expression { get; }
    public string? MemberName { get; }
    public string? FilePath { get; }
    public int LineNumber { get; }
}
```

Custom assertion extensions should preserve `assertion.Context` when returning the original assertion or refining it to another type.

Do not replace the context with a new empty context unless losing the original call-site information is intentional.

---

## Assertion Policies

Policies implement:

```csharp
IAssertionPolicy
```

The policy interface distinguishes three failure categories:

```csharp
public interface IAssertionPolicy
{
    static abstract void Fail(
        AssertionContext context,
        string message);

    static abstract void FailNull(
        AssertionContext context,
        string message);

    static abstract void FailOutOfRange<T>(
        AssertionContext context,
        T actualValue,
        string message);
}
```

The built-in policies are:

```csharp
IsPolicy
GuardPolicy
```

`IsPolicy` maps every assertion failure to:

```csharp
AssertException
```

`GuardPolicy` maps failures onto the standard .NET argument exception hierarchy:

```text
Fail(...)
    → ArgumentException

FailNull(...)
    → ArgumentNullException

FailOutOfRange(...)
    → ArgumentOutOfRangeException
```

Because policy methods are static abstract members and policies are value types, no policy object needs to be allocated or virtually dispatched.

---

## Implementing a Custom Policy

Create a custom policy when the **failure semantics** need to change.

Examples include domain-specific exceptions, protocol validation, parser failures, or integration with another test framework.

```csharp
public readonly struct DomainPolicy : IAssertionPolicy
{
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(
        AssertionContext context,
        string message)
    {
        throw new DomainValidationException(
            message,
            context);
    }

    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(
        AssertionContext context,
        string message)
    {
        throw new DomainValidationException(
            message,
            context);
    }

    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(
        AssertionContext context,
        T actualValue,
        string message)
    {
        throw new DomainValidationException(
            $"{message} Actual value: {actualValue}",
            context);
    }
}
```

The policy should normally remain stateless.

---

## Creating an Entry Point for a Custom Policy

A custom policy normally gets its own small extension method.

Capture the same call-site information as `Is()` and `Guard()`:

```csharp
public static class DomainAssertionEntryExtensions
{
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, DomainPolicy> DomainAssert<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        return new Assertion<T, DomainPolicy>(
            subject,
            new AssertionContext(
                expression,
                memberName,
                filePath,
                lineNumber));
    }
}
```

Existing assertions can immediately be reused:

```csharp
order.Total.DomainAssert()
           .Greater(0);

name.DomainAssert()
    .NotNullOrWhiteSpace();

quantity.DomainAssert()
        .Range(1, 100);
```

Only the failure policy changes.

---

## Writing Custom Assertion Extensions

Most application-specific behavior should be implemented as an **assertion extension**, not as a policy.

Use a policy when you want to change:

> What happens when a condition fails?

Use an assertion extension when you want to change:

> What condition is being checked?

Suppose an application defines:

```csharp
public readonly record struct Percentage(int Value);
```

and every percentage must be between `0` and `100`.

A custom assertion can be implemented once:

```csharp
public static class PercentageAssertions
{
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Percentage, TPolicy> Valid<TPolicy>(
        this Assertion<Percentage, TPolicy> assertion,
        string? message = null)
        where TPolicy : struct, IAssertionPolicy
    {
        var value = assertion.Value;

        if (value.Value is < 0 or > 100)
        {
            TPolicy.FailOutOfRange(
                assertion.Context,
                value,
                message ?? "Percentage must be in range [0, 100].");
        }

        return assertion;
    }
}
```

Usage as an invariant:

```csharp
percentage.Is().Valid();
```

and without another implementation as a guard:

```csharp
percentage.Guard().Valid();
```

The rule exists only once.

---

## Preserving Assertion Context in Extensions

A custom assertion should preserve the current assertion and its context whenever possible.

The normal pattern is:

```csharp
public static Assertion<T, TPolicy> MyAssertion<T, TPolicy>(
    this Assertion<T, TPolicy> assertion,
    string? message = null)
    where TPolicy : struct, IAssertionPolicy
{
    if (/* invalid */)
    {
        TPolicy.Fail(
            assertion.Context,
            message ?? "Failure message.");
    }

    return assertion;
}
```

This preserves:

- the subject
- the selected policy
- the original expression
- member, file, and line information
- fluent chaining

Do not start a new `Is()` or `Guard()` chain inside an assertion extension.

Avoid:

```csharp
public static Assertion<MyType, TPolicy> Valid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion)
    where TPolicy : struct, IAssertionPolicy
{
    assertion.Value.Is().Satisfy(...);

    return assertion;
}
```

That would create a new `IsPolicy` assertion and discard the original policy/context semantics.

Instead, evaluate the rule directly and invoke `TPolicy`.

---

## Example: Domain-Specific Extension

Suppose a mesh is considered valid only if it contains vertices.

```csharp
public static class MeshAssertions
{
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Mesh, TPolicy> HaveVertices<TPolicy>(
        this Assertion<Mesh, TPolicy> assertion,
        string? message = null)
        where TPolicy : struct, IAssertionPolicy
    {
        var mesh = assertion.Value;

        if (mesh.Vertices.Count == 0)
        {
            TPolicy.Fail(
                assertion.Context,
                message ?? "Mesh must contain at least one vertex.");
        }

        return assertion;
    }
}
```

It can now be used as a runtime assertion:

```csharp
mesh.Is().HaveVertices();
```

or as a guard:

```csharp
public void Process(Mesh mesh)
{
    mesh.Guard().HaveVertices();

    // ...
}
```

The selected policy determines the exception type.

---

## Example: Refining the Assertion Type

Extensions can refine the assertion type.

Imagine:

```csharp
Animal animal = GetAnimal();
```

A specialized assertion can validate the runtime type and return:

```csharp
Assertion<Dog, TPolicy>
```

while preserving the original context:

```csharp
public static Assertion<Dog, TPolicy> Dog<TPolicy>(
    this Assertion<Animal, TPolicy> assertion,
    string? message = null)
    where TPolicy : struct, IAssertionPolicy
{
    if (assertion.Value is not Dog dog)
    {
        TPolicy.Fail(
            assertion.Context,
            message ?? "Animal must be a dog.");
    }

    return new Assertion<Dog, TPolicy>(
        dog,
        assertion.Context);
}
```

This enables:

```csharp
animal.Is()
      .Dog()
      .Satisfy(static dog => dog.HasValidChip);
```

without casts in application code.

The built-in `OfType<TExpected>()` follows the same refinement principle.

---

## Extension Design Guidelines

### Keep policies generic

Prefer:

```csharp
public static Assertion<MyType, TPolicy> Valid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion)
    where TPolicy : struct, IAssertionPolicy
```

instead of coupling the rule to one policy:

```csharp
public static Assertion<MyType, IsPolicy> Valid(
    this Assertion<MyType, IsPolicy> assertion)
```

unless the assertion intentionally only makes sense for one failure model.

### Return the assertion

Return the current assertion when the asserted type does not change:

```csharp
return assertion;
```

This preserves chaining:

```csharp
value.Is()
     .Valid()
     .Satisfy(...);
```

### Refine types when validation proves something

If a check proves that:

```csharp
T?
```

is actually:

```csharp
T
```

return:

```csharp
Assertion<T, TPolicy>
```

Likewise for runtime type checks.

When creating the refined wrapper, preserve:

```csharp
assertion.Context
```

### Use the correct failure category

Use:

```csharp
TPolicy.Fail(...)
```

for general failures.

Use:

```csharp
TPolicy.FailNull(...)
```

when the failure specifically means that a value must not be null.

Use:

```csharp
TPolicy.FailOutOfRange(...)
```

for comparison or range violations.

The distinction matters especially for `GuardPolicy`, because it selects the matching standard .NET exception type.

### Accept an optional custom message

Built-in assertions follow this pattern:

```csharp
string? message = null
```

Custom assertions should generally do the same:

```csharp
public static Assertion<MyType, TPolicy> Valid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion,
    string? message = null)
    where TPolicy : struct, IAssertionPolicy
```

Create the default message only in the failure branch.

### Keep the successful path cheap

Assertions are expected to be used frequently.

Prefer direct checks and early returns.

Avoid unnecessary:

```text
LINQ
closures
reflection
stack walking
temporary collections
exception creation
diagnostic formatting
```

on the successful path.

---

# Appendix

## Performance Model

The library deliberately optimizes the successful path.

`Assertion<T, TPolicy>` and `AssertionContext` are `readonly struct` values.

Policies are stateless structs implementing static abstract interface members.

The normal successful path is conceptually:

```text
value
  ↓
Is() / Guard()
  ↓
construct AssertionContext
  ↓
construct small Assertion<T, TPolicy>
  ↓
evaluate condition
  ↓
return assertion
```

The core design does not require:

- policy instances
- virtual dispatch
- reflection
- stack walking
- exception allocation
- heap-allocated fluent builder objects

Failure paths are intentionally allowed to perform more work because they already result in an exception.

Default failure-message formatting and caller-context formatting occur on the failure path.

The priority is:

```text
successful assertion
    → minimal overhead

failed assertion
    → maximum diagnostic quality
```

### Predicate assertions

The predicate overload of `Satisfy()` takes a `Func<T, bool>`.

A non-capturing static lambda is the preferred form when possible:

```csharp
value.Is().Satisfy(static value => value > 0);
```

A capturing lambda can require a closure allocation:

```csharp
value.Is().Satisfy(value => value > minimum);
```

When external state is simple and the expression remains readable, the Boolean overload can be the cheaper choice:

```csharp
value.Is().Satisfy(value > minimum);
```

---

## Choosing Between Specialized Assertions and `Satisfy()`

For one-off conditions, `Satisfy()` is useful:

```csharp
mesh.Is().Satisfy(mesh.Vertices.Count > 0);
```

or:

```csharp
mesh.Is().Satisfy(static mesh => mesh.Vertices.Count > 0);
```

For reusable domain rules, prefer an extension:

```csharp
mesh.Is().HaveVertices();
```

A dedicated extension:

```text
HaveVertices()
    ├── communicates intent
    ├── centralizes the rule
    ├── provides a focused failure message
    ├── works with Is()
    ├── works with Guard()
    └── works with custom policies
```

---

## Recommended Usage

Use `Guard()` at API boundaries:

```csharp
public Mesh Transform(
    Mesh? mesh,
    Matrix4x4 transform,
    float tolerance)
{
    mesh.AsGuardNotNull();
    tolerance.Guard().Greater(0);

    // mesh is known to be non-null here
    // ...
}
```

Use `Is()` for internal assumptions and invariants:

```csharp
var result = ComputeResult();

result.AsNotNull();
result.Vertices.Is().NotEmpty();
```

Use fluent null refinement when the validated value stays inside the chain:

```csharp
GetOptionalName()
    .Is()
    .NotNull()
    .NotEmpty();
```

Create domain-specific extensions for repeated rules:

```csharp
mesh.Guard()
    .HaveVertices()
    .Processable();
```

Create a custom policy only when the **failure semantics** need to change.

---

## API Overview

| Category | Assertions |
| --- | --- |
| Entry points | `Is()`, `Guard()` |
| Flow-aware null entry points | `AsNotNull()`, `AsGuardNotNull()`, `AsNotNullOrEmpty()`, `AsNotNullOrWhiteSpace()`, `AsNotEmpty()`, `AsGuardNotNullOrEmpty()`, `AsGuardNotNullOrWhiteSpace()`, `AsGuardNotEmpty()` |
| Equality | `Eq()`, `NotEq()` |
| Boolean | `True()`, `False()` |
| Arbitrary condition | `Satisfy(bool)`, `Satisfy(Func<T, bool>)` |
| Nullability | `NotNull()`, `Null()` |
| Comparison | `Greater()`, `GreaterEq()`, `Less()`, `LessEq()`, `Range()` |
| Numbers | `Positive()`, `NonNegative()`, `Negative()`, `Zero()`, `NonZero()`, `Even()`, `Odd()`, `DivisibleBy()` |
| Floating point | `Approx()`, `NotNaN()`, `Finite()`, `Infinite()`, `PositiveInfinity()`, `NegativeInfinity()` |
| Strings | `NotNullOrEmpty()`, `NotNullOrWhiteSpace()`, `NotEmpty()`, `Length()`, `MinLength()`, `MaxLength()`, `LengthInRange()`, `Contains()`, `StartsWith()`, `EndsWith()`, `Matches()` |
| Collections | `NotEmpty()`, `Count()`, `Length()`, `MinCount()`, `MaxCount()`, `CountInRange()`, `MinLength()`, `MaxLength()`, `LengthInRange()`, `Contains()` |
| Dictionaries | `ContainsKey()`, `NotContainsKey()` |
| Date and time | `Kind()`, `Utc()`, `After()`, `Before()`, `NotBefore()`, `NotAfter()` (DateTime); `Positive()`, `NonNegative()`, `Zero()`, `WithinTimeout()` (TimeSpan); `Today()`, `Weekday()` (DateOnly); `Utc()`, `HaveOffset()` (DateTimeOffset) |
| GUID | `NotBeEmpty()`, `NotBeNullOrEmpty()` |
| Characters | `IsLetter()`, `IsDigit()`, `IsWhiteSpace()`, `IsUpper()`, `IsLower()` |
| Paths | `AsFile()`, `AsDirectory()`, `Exists()`, `NotExists()`, `HaveExtension()`, `NoExtension()`, `HaveFileName()`, `HaveName()`, `Absolute()`, `HaveFullPath()`, `InDirectory()`, `Empty()`, `NotEmpty()`, `Length()`, `MinLength()`, `MaxLength()`, `LengthInRange()`, `ContainsFile()`, `ContainsDirectory()` |
| URIs | `AsUri()`, `Absolute()`, `HaveScheme()`, `HaveHost()`, `HavePort()`, `Loopback()` |
| Network | `Loopback()`, `IPv4()`, `IPv6()` (IPAddress); `HavePort()`, `Loopback()` (IPEndPoint); `AsMailAddress()`, `HaveHost()`, `HaveUser()` (MailAddress) |
| Runtime type | `OfType<T>()` |

All assertion methods support the selected `TPolicy`, so the same rule can normally be used through `Is()`, `Guard()`, or a custom policy entry point.

---

## Summary

The core idea of `ValidSphere` is simple:

```csharp
value.Is().Valid();
value.Guard().Valid();
```

The validation rule is implemented once.

The policy decides how failure is represented.

```text
Assertion extension
        │
        │ describes
        ▼
   What is valid?
        │
        ▼
      TPolicy
        │
        │ describes
        ▼
 What happens on failure?
```

`AssertionContext` carries compiler-provided call-site information through the chain.

The result is an assertion API that is:

- composable
- reusable
- cheap on the successful path
- easy to debug
- nullable-flow-aware where needed
- independent from a particular exception model
- extensible for application-specific rules
- usable both for runtime invariants and API guards
- usable directly in xUnit.net v3 through `AssertException`
