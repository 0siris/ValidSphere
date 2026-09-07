# Assertions

A lightweight, extensible assertion and guard library for .NET 11.

`Assertions` provides a small fluent API for validating runtime invariants, method arguments, preconditions, and values without introducing a large assertion framework.

The same assertion extensions can be used with different **failure policies**:

```csharp
value.Should().Be(expected);
value.Guard().Be(expected);
```

The assertion logic stays the same. Only the failure behavior changes:

- `Should()` represents a runtime assertion and throws `AssertException`.
- `Guard()` represents argument or precondition validation and throws standard .NET argument exceptions.

This keeps validation rules reusable while preserving the correct failure semantics for each call site.

---

## Goals

The library is designed around a few principles:

- **Small API surface**
- **Fluent but lightweight syntax**
- **No heap allocation on the successful assertion path**
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
  ├── Should()
  │      └── ShouldPolicy
  │             └── AssertException
  │
  └── Guard()
         └── GuardPolicy
                ├── ArgumentException
                ├── ArgumentNullException
                └── ArgumentOutOfRangeException
```

---

## Installation

Add the project or package reference to the consuming project.

For a source/project reference:

```xml
<ItemGroup>
    <ProjectReference Include="..\extern\Assertions\Assertions.csproj" />
</ItemGroup>
```

Then import the namespace:

```csharp
using Assertions;
```

---

## Basic Usage

### Runtime assertions with `Should()`

Use `Should()` when a failed condition represents an invalid program state, violated invariant, unexpected result, or failed assertion.

```csharp
var count = 5;

count.Should().Be(5);
count.Should().BeGreaterThan(0);
```

A failed `Should()` assertion throws `AssertException`.

```csharp
var count = 3;

count.Should().Be(5);
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
items.Count.Should().BeGreaterThan(0);
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
    retryCount.Guard().BeInRange(0, 10);

    // ...
}
```

The important difference from `Should()` is the exception type.

For example:

```csharp
name.Guard().NotBeNull();
```

throws `ArgumentNullException`, while:

```csharp
count.Guard().BeGreaterThan(0);
```

throws `ArgumentOutOfRangeException`.

Other guard failures use `ArgumentException`.

The captured subject expression is used as the parameter name where possible, so:

```csharp
retryCount.Guard().BeGreaterThan(0);
```

can produce an exception with:

```text
ParamName: retryCount
```

This allows public APIs to use fluent assertions while still following normal .NET argument-exception conventions.

If the guarded nullable reference is used again after the check and should participate in C# nullable flow analysis, prefer `GuardNotNull()` as described in [Flow-analysis-aware null checks](#flow-analysis-aware-null-checks).

---

## `Should()` vs. `Guard()`

Both entry points use the same assertion extensions.

```csharp
value.Should().BeGreaterThan(0);
value.Guard().BeGreaterThan(0);
```

The difference is the selected policy.

| Entry point | Intended use | Failure |
| --- | --- | --- |
| `Should()` | Runtime assertions and invariants | `AssertException` |
| `Guard()` | Arguments and preconditions | Standard .NET argument exceptions |

For example:

```csharp
public Mesh Process(Mesh? mesh, float tolerance)
{
    mesh.GuardNotNull();
    tolerance.Guard().BeGreaterThan(0);

    var result = ProcessInternal(mesh, tolerance);

    result.AssertNotNull();

    return result;
}
```

`Guard()` and `GuardNotNull()` validate the caller contract.

`Should()` and `AssertNotNull()` validate assumptions and invariants made by the implementation.

For null checks, prefer `GuardNotNull()` or `AssertNotNull()` when the original variable is used afterwards and should be refined by nullable flow analysis. Use the fluent `.NotBeNull()` form when the refined assertion value is consumed directly or when the check is part of a fluent chain.

---

## Chaining Assertions

Assertion methods return an assertion object again, so checks can be chained directly.

```csharp
value.Should()
     .BeGreaterThan(0)
     .BeLessThan(100);
```

For strings:

```csharp
name.Should()
    .NotBeNullOrWhiteSpace()
    .NotBeEmpty();
```

The policy and original `AssertionContext` are retained for the complete chain.

Conceptually:

```text
value.Should()
     │
     └── Assertion<int, ShouldPolicy>
              │
              ├── BeGreaterThan(0)
              │
              └── BeLessThan(100)
```

No `And` property is required because every assertion directly returns an assertion that can be continued.

---

## Using the Asserted Value

`Assertion<T, TPolicy>` exposes the asserted value through `Value` and can also be converted implicitly back to `T`.

This allows assertions to be used inline.

```csharp
var count = input.Guard().BeGreaterThan(0).Value;
```

or directly:

```csharp
Process(input.Guard().BeGreaterThan(0));
```

For nullable references there are two useful patterns.

When the validated value is consumed directly, the fluent form works well:

```csharp
var result = Process(input.Guard().NotBeNull());
```

When the original variable is used afterwards, use the flow-analysis-aware entry point:

```csharp
input.GuardNotNull();

var result = Process(input);
```

`Guard().NotBeNull()` refines the returned assertion type.

`GuardNotNull()` additionally refines the nullable state of the original variable.

The same distinction applies to `Should().NotBeNull()` and `AssertNotNull()`.

---

## Common Assertions

### Equality

```csharp
value.Should().Be(expected);
value.Should().NotBe(unexpected);
```

Equality uses:

```csharp
EqualityComparer<T>.Default
```

Reference identity should be represented by a dedicated assertion rather than by changing `Be()` semantics.

### Boolean values

```csharp
isValid.Should().BeTrue();
hasErrors.Should().BeFalse();
```

### Arbitrary conditions

`Satisfy()` can be used when no specialized assertion exists.

```csharp
value.Should().Satisfy(value > minimum);
```

The condition expression is captured by the compiler:

```csharp
result.Should().Satisfy(result.Count == expectedCount);
```

A failed assertion can therefore include:

```text
Condition 'result.Count == expectedCount' was not satisfied.
```

For commonly used conditions, prefer a dedicated extension over `Satisfy()` because it communicates intent more clearly and can provide a better failure message.

---

## Nullability

### Reference types

```csharp
string? value = GetValue();

value.Should().NotBeNull();
value.Should().BeNull();
```

`NotBeNull()` refines:

```csharp
Assertion<string?, TPolicy>
```

to:

```csharp
Assertion<string, TPolicy>
```

so subsequent extensions operate on the non-null type.

```csharp
value.Should()
     .NotBeNull()
     .NotBeEmpty();
```

### Flow-analysis-aware null checks

For reference types, the library additionally provides two direct null-check entry points:

```csharp
value.AssertNotNull();
value.GuardNotNull();
```

These methods have the same runtime null-check semantics as:

```csharp
value.Should().NotBeNull();
value.Guard().NotBeNull();
```

but additionally annotate the input with `[NotNull]`.

This allows C# nullable flow analysis to understand that the **original variable** is non-null after the method returns successfully.

```csharp
string? value = GetValue();

value.GuardNotNull();

Console.WriteLine(value.Length); // no nullable warning
```

With the purely fluent form:

```csharp
string? value = GetValue();

value.Guard().NotBeNull();
```

only the returned `Assertion<string, GuardPolicy>` is refined. The compiler cannot infer from that fluent chain that the original variable `value` is non-null afterwards.

The two direct entry points use the same failure semantics as their corresponding policies:

```text
AssertNotNull()  → runtime invariant      → AssertException
GuardNotNull()   → argument/precondition  → ArgumentNullException
```

Both methods still return `Assertion<T, TPolicy>`, so they can be used inline or as the beginning of a fluent chain:

```csharp
Process(value.GuardNotNull());

value.AssertNotNull()
     .NotBeEmpty();
```

Use the direct methods when the nullable state of the original variable matters after the check.

### Nullable value types

Nullable value types can be refined through the fluent API:

```csharp
int? value = GetValue();

value.Should()
     .NotBeNull()
     .BeGreaterThan(0);
```

After `NotBeNull()`, the assertion contains an `int` rather than `int?`.

---

## Numeric Comparisons

The library uses generic math where possible.

```csharp
value.Should().BeGreaterThan(10);
value.Should().BeGreaterThanOrEqualTo(10);
value.Should().BeLessThan(100);
value.Should().BeLessThanOrEqualTo(100);
```

Ranges are inclusive:

```csharp
value.Should().BeInRange(0, 100);
```

Equivalent condition:

```text
0 <= value <= 100
```

The same assertions can be used as guards:

```csharp
percentage.Guard().BeInRange(0, 100);
```

A failed range guard produces `ArgumentOutOfRangeException`.

---

## Floating-Point Assertions

Floating-point values should normally not be compared using exact equality when rounding error is expected.

Use `BeApproximately()` instead:

```csharp
double result = Calculate();

result.Should().BeApproximately(
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
    .Should()
    .BeApproximately(0.3, 1e-12);
```

Negative or `NaN` tolerances are invalid and result in `ArgumentOutOfRangeException`.

---

## Strings

Check for null or empty:

```csharp
string? name = GetName();

name.Should().NotBeNullOrEmpty();
```

Check for null, empty, or whitespace:

```csharp
name.Should().NotBeNullOrWhiteSpace();
```

After these checks, the returned assertion contains a non-null `string`.

```csharp
name.Should()
    .NotBeNullOrWhiteSpace()
    .NotBeEmpty();
```

For argument validation:

```csharp
public User(string? name)
{
    Name = name.Guard()
               .NotBeNullOrWhiteSpace();
}
```

If the original variable must be recognized as non-null afterwards, use `GuardNotNull()` first.

---

## GUIDs

Require a non-empty GUID:

```csharp
id.Guard().NotBeEmpty();
```

For nullable GUIDs:

```csharp
Guid? id = GetId();

id.Should().NotBeNullOrEmpty();
```

The result is refined to:

```csharp
Assertion<Guid, TPolicy>
```

---

## Collections

Require at least one element:

```csharp
items.Should().NotBeEmpty();
```

Require an exact count:

```csharp
items.Should().HaveCount(3);
```

Arrays additionally support exact length checks:

```csharp
buffer.Should().HaveLength(1024);
```

The collection assertions intentionally operate on collection types that expose a count directly. They do not implicitly enumerate arbitrary `IEnumerable<T>` sequences just to obtain a count.

---

## Runtime Type Assertions

Runtime type checks can refine the assertion type.

```csharp
object value = GetValue();

value.Should().BeOfType<MyType>();
```

After the check, the assertion contains `MyType` rather than `object`.

This allows further type-specific assertions without a separate cast:

```csharp
value.Should()
     .BeOfType<MyType>()
     .BeValid();
```

The refined assertion preserves the original `AssertionContext`.

---

## Failure Messages

Every assertion provides a meaningful default failure message and accepts an optional custom message.

Typical default messages are:

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

A custom message can add domain-specific context:

```csharp
count.Should().BeGreaterThan(
    0,
    "A mesh must contain at least one vertex.");
```

When a custom message is supplied, it replaces the assertion-specific default message. The captured caller information is still appended by the selected failure policy.

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

A string literal does not introduce a per-call string allocation:

```csharp
count.Guard().BeGreaterThan(0, "Count must be positive.");
```

However, an interpolated string is evaluated before the assertion method is called:

```csharp
count.Guard().BeGreaterThan(0, $"Invalid count {count} for {id}.");
```

If dynamic custom messages are used in very hot paths, keep in mind that this can allocate even when the assertion succeeds unless a dedicated interpolated-string-handler overload is provided.

---

## Caller Information and Debugging

`Should()`, `Guard()`, `AssertNotNull()`, and `GuardNotNull()` capture compiler-provided call-site information:

- `CallerArgumentExpression`
- `CallerMemberName`
- `CallerFilePath`
- `CallerLineNumber`

The values are stored in an `AssertionContext` and preserved throughout a fluent chain.

```text
Expression: mesh.Vertices.Count
Member: ProcessMesh
Source: C:\Projects\Example\MeshProcessor.cs(142)
```

Caller information is inserted by the compiler. Capturing it does not require:

- reflection
- stack walking
- source-code parsing
- file I/O

The library also uses debugger/runtime attributes where appropriate:

```csharp
[DebuggerStepThrough]
[StackTraceHidden]
[DoesNotReturn]
```

`DebuggerStepThrough` keeps normal debugger stepping out of assertion infrastructure.

`StackTraceHidden` hides internal assertion frames from normal formatted stack traces where appropriate.

`DoesNotReturn` communicates failure-path control flow to the compiler and analyzers.

The failure policy methods are marked `NoInlining` so the exception construction and diagnostic formatting stay on the cold path, while the small assertion methods are intended to remain inline-friendly.

> `CallerFilePath` can contain an absolute build-machine path. If exceptions are exposed outside trusted diagnostics, consider whether that information should be sanitized by the selected policy.

---

## Test Framework Integration

`Should()` can also be used inside unit tests.

```csharp
[Fact]
public void Calculates_expected_value()
{
    var result = Calculate();

    result.Should().Be(42);
}
```

`AssertException` implements the marker interface:

```csharp
IAssertionException
```

The marker is intentionally declared by this library rather than requiring a direct dependency on a particular test framework. Frameworks that recognize an interface with that name, such as xUnit.net v3, can classify the exception as an assertion failure.

`Guard()` remains intentionally different:

```csharp
value.Guard().BeGreaterThan(0);
```

A failed guard is still an `ArgumentException`, `ArgumentNullException`, or `ArgumentOutOfRangeException`, because it represents a violated API contract rather than an assertion failure.

If another environment requires different exception semantics, add a custom policy rather than changing the assertion implementations.

---

## Architecture

The core architecture consists of four concepts:

```text
             AssertionContext
                    │
                    ▼
          Assertion<T, TPolicy>
                    │
          ┌─────────┴─────────┐
          │                   │
 Assertion extensions       TPolicy
          │                   │
   Be / NotBe / ...      Failure behavior
                              │
                      ┌───────┴────────┐
                      │                │
                 ShouldPolicy     GuardPolicy
```

An assertion extension defines **what condition is valid**.

The policy defines **what happens when the condition fails**.

The `AssertionContext` carries the original call-site information through the chain.

For example, `BeGreaterThan(0)` does not need to know whether it is being used as:

```csharp
value.Should().BeGreaterThan(0);
```

or:

```csharp
value.Guard().BeGreaterThan(0);
```

The extension evaluates the condition and delegates failures to `TPolicy`.

---

## `AssertionContext`

`AssertionContext` is a lightweight `readonly struct` containing compiler-provided source information:

```csharp
public readonly struct AssertionContext
{
    public string? Expression { get; }
    public string? MemberName { get; }
    public string? FilePath { get; }
    public int LineNumber { get; }
}
```

The context is captured once at the entry point:

```csharp
value.Should()
value.Guard()
value.AssertNotNull()
value.GuardNotNull()
```

and then carried through every subsequent assertion.

Custom assertions should always reuse `assertion.Context` rather than creating a new context.

---

## Assertion Policies

Policies implement `IAssertionPolicy`.

A policy controls what happens when an assertion fails.

The built-in policies are:

```csharp
ShouldPolicy
GuardPolicy
```

`ShouldPolicy` maps assertion failures to `AssertException`.

`GuardPolicy` maps them to the appropriate standard .NET argument exception.

The policy contract distinguishes different failure categories:

```csharp
public interface IAssertionPolicy
{
    [DoesNotReturn]
    static abstract void Fail(
        AssertionContext context,
        string message);

    [DoesNotReturn]
    static abstract void FailNull(
        AssertionContext context,
        string message);

    [DoesNotReturn]
    static abstract void FailOutOfRange<T>(
        AssertionContext context,
        T actualValue,
        string message);
}
```

This allows:

```csharp
value.Guard().NotBeNull();
```

to produce `ArgumentNullException`, while:

```csharp
value.Guard().BeGreaterThan(0);
```

can produce `ArgumentOutOfRangeException`.

The same assertion implementation works with both policies.

---

## Implementing a Custom Policy

Create a custom policy when the **failure representation** needs to change.

Typical use cases include:

- test frameworks
- domain-specific exceptions
- protocol validation
- parser errors
- validation pipelines
- diagnostic frameworks

A policy should normally be a stateless value type:

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

The policy contains no instance state.

Policy behavior is selected through the generic parameter:

```csharp
Assertion<T, DomainPolicy>
```

No policy object needs to be allocated or stored in the assertion.

---

## Creating an Entry Point for a Custom Policy

A custom policy normally gets its own small extension method.

The entry point should capture the same compiler-provided caller information as `Should()` and `Guard()`:

```csharp
public static class DomainAssertionExtensions
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
        var context = new AssertionContext(
            expression,
            memberName,
            filePath,
            lineNumber);

        return new Assertion<T, DomainPolicy>(subject, context);
    }
}
```

Usage:

```csharp
order.Total.DomainAssert()
           .BeGreaterThan(0);
```

Existing assertions can immediately be reused:

```csharp
name.DomainAssert()
    .NotBeNullOrWhiteSpace();

id.DomainAssert()
  .NotBeEmpty();

quantity.DomainAssert()
        .BeInRange(1, 100);
```

No copy of the actual assertion logic is necessary. Only the failure policy changes.

---

## Writing Custom Assertion Extensions

Most application-specific behavior should be implemented as an **assertion extension**, not as a new policy.

Use a policy when you want to change:

> What happens when a condition fails?

Use an assertion extension when you want to change:

> What condition is being checked?

For example:

```csharp
public readonly record struct Percentage(int Value);
```

A reusable assertion can be written as:

```csharp
public static class PercentageAssertions
{
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Percentage, TPolicy> BeValid<TPolicy>(
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

Usage:

```csharp
percentage.Should().BeValid();
percentage.Guard().BeValid();
```

The rule exists only once. The selected policy determines the resulting failure semantics.

---

## Preserving Assertion Context in Extensions

A custom extension should preserve the current assertion and its `AssertionContext` whenever possible.

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
- the policy
- the original caller expression
- the caller member
- the source file
- the source line
- fluent chaining

Do **not** start a new `Should()` or `Guard()` chain inside an assertion extension.

Avoid:

```csharp
public static Assertion<MyType, TPolicy> BeValid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion)
    where TPolicy : struct, IAssertionPolicy
{
    assertion.Value.Should().Satisfy(...);

    return assertion;
}
```

That would discard the original policy and call-site context. A `Guard()` chain could unexpectedly become a `Should()` assertion.

Instead, keep `TPolicy` and pass `assertion.Context` directly to the policy.

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
mesh.Should().HaveVertices();
```

or as a guard:

```csharp
public void Process(Mesh mesh)
{
    mesh.Guard().HaveVertices();

    // ...
}
```

The check is implemented once. The selected policy determines the exception type.

---

## Example: Refining the Assertion Type

Assertions can also prove a more specific type and return a refined assertion.

Imagine:

```csharp
Animal animal = GetAnimal();
```

A specialized assertion can validate the runtime type and return `Assertion<Dog, TPolicy>`:

```csharp
public static Assertion<Dog, TPolicy> BeDog<TPolicy>(
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
animal.Should()
      .BeDog()
      .HaveValidChip();
```

without casts in application code.

The built-in `BeOfType<T>()` follows the same principle.

---

## Extension Design Guidelines

### Keep policies generic

Prefer:

```csharp
public static Assertion<MyType, TPolicy> BeValid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion,
    string? message = null)
    where TPolicy : struct, IAssertionPolicy
```

instead of:

```csharp
public static Assertion<MyType, ShouldPolicy> BeValid(
    this Assertion<MyType, ShouldPolicy> assertion)
```

unless the assertion intentionally only makes sense for one policy.

This makes the extension automatically usable with:

```csharp
Should()
Guard()
```

and custom policies.

### Return the assertion

Prefer:

```csharp
return assertion;
```

This allows:

```csharp
value.Should()
     .BeValid()
     .HaveSomething()
     .Satisfy(...);
```

### Refine types when validation proves something

If an assertion proves that `T?` is actually `T`, return:

```csharp
Assertion<T, TPolicy>
```

rather than retaining the nullable assertion type.

Likewise for runtime type checks.

When constructing a refined assertion, always preserve:

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

when the subject must not be null.

Use:

```csharp
TPolicy.FailOutOfRange(...)
```

for range violations.

The distinction matters especially for `GuardPolicy`, because it maps these categories onto different standard .NET exception types.

### Accept an optional custom message

For consistency with the built-in API, reusable assertion extensions should normally accept:

```csharp
string? message = null
```

and use:

```csharp
message ?? "Default failure message."
```

inside the failure branch.

### Keep the successful path cheap

Assertions are expected to be used frequently.

Prefer code where diagnostic work is performed only after the condition has failed:

```csharp
if (condition)
    return assertion;

TPolicy.Fail(
    assertion.Context,
    message ?? "Failure message.");
```

Avoid unnecessary allocations, LINQ, closures, reflection, stack walking, or exception creation on the successful path.

---

## Performance Model

The library deliberately optimizes the successful path.

`Assertion<T, TPolicy>` and `AssertionContext` are `readonly struct` types, and policies are stateless structs implementing static abstract interface members.

The normal successful path is conceptually:

```text
value
  ↓
capture compiler-provided caller metadata
  ↓
construct small assertion structs
  ↓
evaluate condition
  ↓
return same assertion / refined assertion
```

There is no requirement for:

- policy instances
- virtual dispatch
- reflection
- stack-trace inspection
- fluent builder objects on the heap
- exception allocation

The small entry points and assertion methods are marked to be inline-friendly.

Failure helpers are intentionally kept on the cold path and may perform more work because a failure already results in an exception.

The design priority is therefore:

```text
successful assertion
    → minimal overhead

failed assertion
    → maximum diagnostic quality
```

---

## Choosing Between Specialized Assertions and `Satisfy()`

For one-off conditions, `Satisfy()` is useful:

```csharp
mesh.Should().Satisfy(mesh.Vertices.Count > 0);
```

For reusable domain rules, prefer an extension:

```csharp
mesh.Should().HaveVertices();
```

A dedicated extension:

```text
HaveVertices()
    ├── communicates intent
    ├── centralizes the rule
    ├── provides a better failure message
    ├── preserves AssertionContext
    ├── works with Should()
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
    mesh.GuardNotNull();
    tolerance.Guard().BeGreaterThan(0);

    // mesh is non-null here
    // ...
}
```

Use `Should()` for internal assumptions and invariants:

```csharp
var result = ComputeResult();

result.AssertNotNull();
result.Vertices.Should().NotBeEmpty();
```

Use the fluent null assertion when the refined value is consumed through the chain:

```csharp
return GetResult()
    .Should()
    .NotBeNull()
    .BeValid();
```

Create domain-specific extensions for repeated rules:

```csharp
mesh.Guard()
    .HaveVertices()
    .BeProcessable();
```

Create a custom policy only when the **failure semantics** need to change.

---

## Summary

The central idea of `Assertions` is simple:

```csharp
value.Should().BeValid();
value.Guard().BeValid();
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

`AssertionContext` preserves compiler-provided call-site information through the complete chain.

For nullable reference variables that must remain refined after the call, the direct flow-analysis-aware entry points are available:

```csharp
value.AssertNotNull();
value.GuardNotNull();
```

This keeps assertions:

- composable
- reusable
- fast on the successful path
- easy to debug
- compatible with nullable flow analysis
- independent from a particular exception model
- extensible for application-specific rules
- usable both for runtime invariants and API guards