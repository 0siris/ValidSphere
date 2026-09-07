# Assertions

A lightweight, extensible assertion and guard library for .NET.

`Assertions` provides a small fluent API for validating values, runtime invariants, method arguments, and preconditions without introducing a large assertion framework.

The same assertion extensions can be used with different **failure policies**:

```csharp
value.Should().Be(expected);
value.Guard().Be(expected);
```

The assertion itself stays the same. Only the failure behavior changes.

* `Should()` represents a runtime assertion and throws `AssertException`.
* `Guard()` represents argument/precondition validation and throws standard .NET argument exceptions.

This makes assertion logic reusable while keeping failure semantics appropriate for the context in which it is used.

---

## Goals

The library is designed around a few principles:

* **Small API surface**
* **Fluent but lightweight syntax**
* **No heap allocation on the successful assertion path**
* **No reflection**
* **No virtual dispatch for assertion policies**
* **Reusable assertion extensions**
* **Separate failure semantics through policies**
* **Good compiler and debugger integration**
* **Caller expressions captured automatically**
* **Assertions can be embedded directly into expressions**
* **Easy to extend with project-specific assertions and policies**

The core assertion object is a `readonly struct`, while policies use static abstract interface members.

This allows the compiler to resolve policy behavior statically:

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

# Installation

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

# Basic Usage

## Runtime assertions with `Should`

Use `Should()` when a failed condition represents an invalid program state, violated invariant, unexpected result, or failed assertion.

```csharp
var count = 5;

count.Should().Be(5);
count.Should().BeGreaterThan(0);
```

A failed `Should()` assertion throws an `AssertException`.

Example:

```csharp
var count = 3;

count.Should().Be(5);
```

produces an assertion failure similar to:

```text
Expected '5', but found '3'.
Expression: count
```

The original source expression is captured automatically through `CallerArgumentExpression`.

That also works for expressions:

```csharp
items.Count.Should().BeGreaterThan(0);
```

The assertion can therefore report:

```text
Expression: items.Count
```

instead of only reporting the resulting value.

---

# Guards

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

throws:

```csharp
ArgumentNullException
```

while:

```csharp
count.Guard().BeGreaterThan(0);
```

throws:

```csharp
ArgumentOutOfRangeException
```

Other guard failures use:

```csharp
ArgumentException
```

This means public APIs can use the fluent assertion syntax while still following the normal .NET exception conventions.

---

# `Should` vs. `Guard`

Both entry points use the same assertion extensions.

```csharp
value.Should().BeGreaterThan(0);
value.Guard().BeGreaterThan(0);
```

The difference is only the selected policy.

| Entry point | Intended use                      | Failure                           |
| ----------- | --------------------------------- | --------------------------------- |
| `Should()`  | Runtime assertions and invariants | `AssertException`                 |
| `Guard()`   | Arguments and preconditions       | Standard .NET argument exceptions |

For example:

```csharp
public Mesh Process(Mesh mesh, float tolerance)
{
    mesh.Guard().NotBeNull();
    tolerance.Guard().BeGreaterThan(0);

    var result = ProcessInternal(mesh, tolerance);

    result.Should().NotBeNull();

    return result;
}
```

`Guard()` validates the caller contract.

`Should()` validates assumptions made by the implementation.

---

# Chaining Assertions

Assertion methods return the assertion object again, so checks can be chained.

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

The policy is retained for the complete chain.

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

# Using the Asserted Value

`Assertion<T, TPolicy>` exposes the asserted value and can also be converted implicitly back to `T`.

This allows assertions to be used inline.

For example:

```csharp
var count = input.Guard().BeGreaterThan(0).Value;
```

or directly:

```csharp
Process(input.Guard().BeGreaterThan(0));
```

This is especially useful for guards.

Instead of:

```csharp
input.Guard().NotBeNull();

var result = Process(input);
```

you can write:

```csharp
var result = Process(input.Guard().NotBeNull());
```

Null assertions also refine the resulting type.

For example, given:

```csharp
string? name = GetName();
```

this:

```csharp
var validatedName = name.Guard().NotBeNull();
```

produces an assertion over a non-null `string`.

The same principle applies to nullable value types and runtime type assertions.

---

# Common Assertions

## Equality

```csharp
value.Should().Be(expected);
value.Should().NotBe(unexpected);
```

Equality uses:

```csharp
EqualityComparer<T>.Default
```

---

## Boolean values

```csharp
isValid.Should().BeTrue();
hasErrors.Should().BeFalse();
```

---

## Arbitrary conditions

`Satisfy` can be used when no specialized assertion exists.

```csharp
value.Should().Satisfy(value > minimum);
```

Because the condition expression is captured by the compiler, a failed assertion can identify the original condition.

```csharp
result.Should().Satisfy(result.Count == expectedCount);
```

instead of reporting only:

```text
Condition was not satisfied.
```

the failure can include:

```text
Condition 'result.Count == expectedCount' was not satisfied.
```

For commonly used conditions, prefer a dedicated extension over `Satisfy()` because it provides better semantics and better failure messages.

---

# Nullability

## Reference types

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

Example:

```csharp
value.Should()
     .NotBeNull()
     .NotBeEmpty();
```

---

## Nullable value types

```csharp
int? value = GetValue();

value.Should().NotBeNull()
     .BeGreaterThan(0);
```

After `NotBeNull()`, the assertion contains an `int` rather than `int?`.

---

# Numeric Comparisons

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

A failed range guard results in an `ArgumentOutOfRangeException`.

---

# Floating-Point Assertions

Floating-point values should normally not be compared using exact equality.

Use `BeApproximately` instead:

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

Negative or `NaN` tolerances are invalid and result in an `ArgumentOutOfRangeException`.

---

# Strings

Check for null or empty:

```csharp
string? name = GetName();

name.Should().NotBeNullOrEmpty();
```

Check for null, empty, or whitespace:

```csharp
name.Should().NotBeNullOrWhiteSpace();
```

After these checks the returned assertion contains a non-null `string`.

This allows:

```csharp
name.Should()
    .NotBeNullOrWhiteSpace()
    .NotBeEmpty();
```

For argument validation:

```csharp
public User(string name)
{
    Name = name.Guard()
               .NotBeNullOrWhiteSpace();
}
```

---

# GUIDs

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

# Collections

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

---

# Runtime Type Assertions

Runtime type checks can refine the assertion type.

```csharp
object value = GetValue();

value.Should().BeOfType<MyType>();
```

After the check the assertion contains:

```csharp
MyType
```

rather than:

```csharp
object
```

This allows further type-specific assertions.

---

# Failure Messages

Assertions provide meaningful default failure messages.

Typical examples are:

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

Where an assertion provides an optional custom message, it can be used to add domain-specific context.

For example:

```csharp
count.Should().BeGreaterThan(
    0,
    "A mesh must contain at least one vertex.");
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

The latter information is already provided by the assertion itself.

---

# Caller Information and Debugging

The assertion entry points capture source information automatically.

This allows failures to identify the original call site instead of requiring callers to provide diagnostic information manually.

Depending on the assertion context, diagnostics can contain information such as:

```text
Expression: mesh.Vertices.Count
Member: ProcessMesh
File: MeshProcessor.cs
Line: 142
```

Caller information is supplied by the compiler and therefore does not require stack walking on the successful assertion path.

Failure helpers are hidden from stack traces where appropriate so that debugging points at application code rather than internal assertion infrastructure.

The implementation also uses debugger/runtime attributes such as:

```csharp
[StackTraceHidden]
[DebuggerStepThrough]
[DoesNotReturn]
```

where appropriate to keep failure stacks and debugging behavior focused on the actual call site.

---

# Architecture

The core architecture consists of three parts:

```text
              Assertion<T, TPolicy>
                       │
             ┌─────────┴─────────┐
             │                   │
       Assertion Extensions     TPolicy
             │                   │
       Be / NotBe / ...     Failure behavior
                                 │
                         ┌───────┴────────┐
                         │                │
                    ShouldPolicy     GuardPolicy
```

The assertion extension contains the **condition**.

The policy contains the **failure behavior**.

This separation is intentional.

For example, the implementation of:

```csharp
BeGreaterThan(0)
```

does not need to know whether it is being used as:

```csharp
value.Should().BeGreaterThan(0);
```

or:

```csharp
value.Guard().BeGreaterThan(0);
```

It simply delegates the failure to `TPolicy`.

---

# Assertion Policies

Policies implement:

```csharp
IAssertionPolicy
```

A policy controls what happens when an assertion fails.

The built-in policies are:

```csharp
ShouldPolicy
GuardPolicy
```

`ShouldPolicy` maps assertion failures to:

```csharp
AssertException
```

`GuardPolicy` maps them to the appropriate standard .NET argument exception.

The policy interface distinguishes different failure categories:

```csharp
public interface IAssertionPolicy
{
    static abstract void Fail(
        string? expression,
        string message);

    static abstract void FailNull(
        string? expression,
        string message);

    static abstract void FailOutOfRange<T>(
        string? expression,
        T actualValue,
        string message);
}
```

This allows an assertion such as:

```csharp
value.Guard().NotBeNull();
```

to produce an `ArgumentNullException`, while:

```csharp
value.Guard().BeGreaterThan(0);
```

can produce an `ArgumentOutOfRangeException`.

---

# Implementing a Custom Policy

Custom policies are useful when assertion failures need to integrate with another environment.

Examples include:

* test frameworks
* domain-specific exceptions
* protocol validation
* parser errors
* validation pipelines
* diagnostic frameworks

A policy should normally be a stateless value type:

```csharp
public readonly struct DomainPolicy : IAssertionPolicy
{
    [DoesNotReturn]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(
        string? expression,
        string message)
    {
        throw new DomainValidationException(
            expression,
            message);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(
        string? expression,
        string message)
    {
        throw new DomainValidationException(
            expression,
            message);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(
        string? expression,
        T actualValue,
        string message)
    {
        throw new DomainValidationException(
            expression,
            $"{message} Actual value: {actualValue}");
    }
}
```

The policy itself contains no state.

This is important because policies are selected through the generic parameter:

```csharp
Assertion<T, DomainPolicy>
```

and their static methods can be resolved without allocating a policy object.

---

# Creating an Entry Point for a Custom Policy

A policy normally gets its own small extension method.

For example:

```csharp
public static class DomainAssertionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, DomainPolicy> DomainAssert<T>(
        this T subject,
        [CallerArgumentExpression("subject")]
        string? expression = null)
    {
        return new Assertion<T, DomainPolicy>(
            subject,
            expression);
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

No copy of the actual assertion logic is necessary.

Only the failure policy changes.

> If caller file, member, and line information are part of the assertion context in the current implementation, custom entry points should forward those compiler-supplied caller values in exactly the same way as `Should()` and `Guard()`.

---

# Test-Framework Policies

The policy abstraction can also be used to integrate the assertions with a test framework.

For example, an xUnit-specific policy could translate failures into the assertion exception type expected by xUnit.

Conceptually:

```csharp
public readonly struct XunitPolicy : IAssertionPolicy
{
    [DoesNotReturn]
    [StackTraceHidden]
    public static void Fail(
        string? expression,
        string message)
    {
        throw CreateXunitException(expression, message);
    }

    // ...
}
```

Then an entry point can expose:

```csharp
actual.Expect().Be(expected);
```

while all existing assertion extensions remain reusable.

This separation avoids coupling the core assertion implementations directly to a specific test framework.

---

# Writing Custom Assertion Extensions

Most application-specific behavior should be implemented as an **assertion extension**, not as a new policy.

Use a policy when you want to change:

> What happens when a condition fails?

Use an assertion extension when you want to change:

> What condition is being checked?

For example, suppose an application has this value:

```csharp
public readonly record struct Percentage(int Value);
```

and every percentage must be between `0` and `100`.

A custom assertion could be:

```csharp
public static class PercentageAssertions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Percentage, TPolicy> BeValid<TPolicy>(
        this Assertion<Percentage, TPolicy> assertion)
        where TPolicy : struct, IAssertionPolicy
    {
        var value = assertion.Value;

        if (value.Value is < 0 or > 100)
        {
            TPolicy.FailOutOfRange(
                null,
                value,
                "Percentage must be in range [0, 100].");
        }

        return assertion;
    }
}
```

Usage:

```csharp
percentage.Should().BeValid();
```

and without writing a second implementation:

```csharp
percentage.Guard().BeValid();
```

The same assertion automatically uses the selected policy.

---

# Preserving Assertion Context in Extensions

A custom extension should preserve the current assertion whenever possible.

The normal pattern is:

```csharp
public static Assertion<T, TPolicy> MyAssertion<T, TPolicy>(
    this Assertion<T, TPolicy> assertion)
    where TPolicy : struct, IAssertionPolicy
{
    if (/* invalid */)
    {
        TPolicy.Fail(
            /* current expression/context */,
            "Failure message.");
    }

    return assertion;
}
```

This preserves:

* the subject
* the policy
* the original caller expression
* diagnostic context
* fluent chaining

Do not start a new `Should()` or `Guard()` chain inside an assertion extension.

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

That would discard the original policy.

A `Guard()` chain could unexpectedly become a `Should()` assertion.

Instead, always keep `TPolicy`.

---

# Example: Domain-Specific Extension

Suppose a mesh is considered valid only if it contains vertices.

```csharp
public static class MeshAssertions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Mesh, TPolicy> HaveVertices<TPolicy>(
        this Assertion<Mesh, TPolicy> assertion)
        where TPolicy : struct, IAssertionPolicy
    {
        var mesh = assertion.Value;

        if (mesh.Vertices.Count == 0)
        {
            TPolicy.Fail(
                null,
                "Mesh must contain at least one vertex.");
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

The check exists only once.

The selected policy determines the resulting exception.

---

# Example: Refining the Assertion Type

Extensions can also refine a type.

Imagine:

```csharp
Animal animal = GetAnimal();
```

A specialized assertion can validate the runtime type and return:

```csharp
Assertion<Dog, TPolicy>
```

This is the same principle used by `BeOfType<T>()`.

A refinement extension should return a new assertion carrying the original assertion context:

```csharp
public static Assertion<Dog, TPolicy> BeDog<TPolicy>(
    this Assertion<Animal, TPolicy> assertion)
    where TPolicy : struct, IAssertionPolicy
{
    if (assertion.Value is not Dog dog)
    {
        TPolicy.Fail(
            null,
            "Animal must be a dog.");
    }

    return new Assertion<Dog, TPolicy>(
        dog,
        /* preserve assertion context */);
}
```

This enables:

```csharp
animal.Should()
      .BeDog()
      .HaveValidChip();
```

without casts in application code.

---

# Extension Design Guidelines

When adding assertions, prefer the following rules.

## Keep policies generic

Write:

```csharp
public static Assertion<MyType, TPolicy> BeValid<TPolicy>(
    this Assertion<MyType, TPolicy> assertion)
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

---

## Return the assertion

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

---

## Refine types when validation proves something

If an assertion proves that:

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

rather than retaining the nullable assertion type.

Likewise for runtime type checks.

This allows the compiler to benefit from information proven by the assertion.

---

## Use the correct failure category

Use:

```csharp
TPolicy.Fail(...)
```

for general failures.

Use:

```csharp
TPolicy.FailNull(...)
```

when the failure specifically means that the subject must not be null.

Use:

```csharp
TPolicy.FailOutOfRange(...)
```

for range violations.

The distinction matters especially for `GuardPolicy`, because it maps these categories onto different standard .NET exception types.

---

## Keep the successful path cheap

Assertions are expected to be used frequently.

Prefer:

```csharp
if (condition)
    return assertion;

TPolicy.Fail(...);
```

or equivalent code where expensive diagnostics are only created on failure.

Avoid unnecessary allocations, LINQ, closures, reflection, or exception creation on the successful path.

---

# Performance Model

The library deliberately optimizes the successful path.

`Assertion<T, TPolicy>` is a:

```csharp
readonly struct
```

and policies are stateless structs implementing static abstract interface members.

The general successful path is therefore conceptually:

```text
value
  ↓
construct small assertion struct
  ↓
evaluate condition
  ↓
return same assertion struct
```

There is no requirement for:

* policy instances
* virtual dispatch
* reflection
* exception allocation
* fluent builder objects on the heap

Failure paths are intentionally allowed to perform more work because they already result in an exception.

The priority is therefore:

```text
successful assertion
    → minimal overhead

failed assertion
    → maximum diagnostic quality
```

---

# Choosing Between Specialized Assertions and `Satisfy`

For one-off conditions, `Satisfy()` is useful:

```csharp
mesh.Should().Satisfy(mesh.Vertices.Count > 0);
```

For reusable domain rules, prefer an extension:

```csharp
mesh.Should().HaveVertices();
```

The extension has several advantages:

```text
HaveVertices()
    ├── communicates intent
    ├── centralizes the rule
    ├── provides a better failure message
    ├── works with Should()
    ├── works with Guard()
    └── works with custom policies
```

---

# Recommended Usage

Use `Guard()` at API boundaries:

```csharp
public Mesh Transform(
    Mesh mesh,
    Matrix4x4 transform,
    float tolerance)
{
    mesh.Guard().NotBeNull();
    tolerance.Guard().BeGreaterThan(0);

    // ...
}
```

Use `Should()` for internal assumptions and invariants:

```csharp
var result = ComputeResult();

result.Should().NotBeNull();
result.Vertices.Should().NotBeEmpty();
```

Create domain-specific extensions for repeated rules:

```csharp
mesh.Guard()
    .HaveVertices()
    .BeProcessable();
```

Create a custom policy only when the **failure semantics** need to change.

---

# Summary

The core idea of `Assertions` is simple:

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

This keeps assertions:

* composable
* reusable
* fast on the successful path
* easy to debug
* independent from a particular exception model
* extensible for application-specific rules
* usable both for runtime invariants and API guards