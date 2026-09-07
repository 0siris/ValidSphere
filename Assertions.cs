using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Assertions2;

/// <summary>
///     Represents a lightweight fluent assertion for a value.
/// </summary>
/// <typeparam name="T">The type of the asserted value.</typeparam>
/// <typeparam name="TPolicy">
///     The failure policy that determines which exception type is thrown when an assertion fails.
/// </typeparam>
/// <remarks>
///     This type is implemented as a <see langword="readonly struct" /> to avoid heap allocations
///     on the successful assertion path. Its public constructor and properties also form the minimal
///     extension surface for custom assertion libraries.
/// </remarks>
public readonly struct Assertion<T, TPolicy>
    where TPolicy : struct, IAssertionPolicy {

    private readonly T subject;
    private readonly string? expression;

    /// <summary>
    ///     Initializes a new assertion for the specified subject.
    /// </summary>
    /// <param name="subject">The value to assert.</param>
    /// <param name="expression">
    ///     The source expression that produced <paramref name="subject" />.
    /// </param>
    /// <remarks>
    ///     This constructor is public so custom assertion extensions can refine an assertion to another type.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Assertion(T subject, string? expression) {
        this.subject = subject;
        this.expression = expression;
    }

    /// <summary>
    ///     Gets the asserted value.
    /// </summary>
    public T Value {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => subject;
    }

    /// <summary>
    ///     Gets the source expression that produced the asserted value.
    /// </summary>
    /// <remarks>
    ///     The expression is captured by <see cref="AssertionEntryExtensions.Should{T}(T, string?)" />
    ///     or <see cref="AssertionEntryExtensions.Guard{T}(T, string?)" /> and can be reused by custom assertions.
    /// </remarks>
    public string? Expression {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => expression;
    }

    /// <summary>
    ///     Asserts that the subject is compatible with <typeparamref name="TExpected" /> and refines
    ///     the assertion to that type.
    /// </summary>
    /// <typeparam name="TExpected">The required runtime type.</typeparam>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion whose subject is typed as <typeparamref name="TExpected" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Assertion<TExpected, TPolicy> BeOfType<TExpected>(string? message = null) {
        if (subject is TExpected typed)
            return new Assertion<TExpected, TPolicy>(typed, expression);

        TPolicy.Fail(expression,
                     message ?? $"Value must be of type '{typeof(TExpected).Name}'.");

        return default;
    }

    /// <summary>
    ///     Implicitly extracts the asserted value.
    /// </summary>
    /// <param name="assertion">The assertion whose value is returned.</param>
    /// <returns>The asserted value.</returns>
    /// <remarks>
    ///     This allows assertions to be embedded directly into expressions, for example:
    ///     <c>Use(value.Guard().BeGreaterThan(0))</c>.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Assertion<T, TPolicy> assertion)
        => assertion.subject;
}

/// <summary>
///     Defines the failure behavior used by fluent assertions.
/// </summary>
/// <remarks>
///     Implementations are stateless value types. Static abstract members allow assertion extensions
///     to select the appropriate failure behavior without reflection, allocation, or virtual dispatch.
/// </remarks>
public interface IAssertionPolicy {
    /// <summary>
    ///     Throws an exception for a general assertion failure.
    /// </summary>
    /// <param name="expression">The expression that produced the asserted value.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void Fail(string? expression, string message);

    /// <summary>
    ///     Throws an exception when an asserted value is unexpectedly <see langword="null" />.
    /// </summary>
    /// <param name="expression">The expression that produced the asserted value.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void FailNull(string? expression, string message);

    /// <summary>
    ///     Throws an exception when an asserted value is outside an allowed range.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <param name="expression">The expression that produced the asserted value.</param>
    /// <param name="actualValue">The value that violated the range constraint.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void FailOutOfRange<T>(string? expression, T actualValue, string message);
}

/// <summary>
///     Marks an exception as an assertion failure for compatible test frameworks.
/// </summary>
/// <remarks>
///     xUnit.net v3 recognizes exceptions implementing an interface named
///     <c>IAssertionException</c> as assertion failures without requiring this library to reference xUnit.net.
/// </remarks>
public interface IAssertionException;

/// <summary>
///     Failure policy used by <see cref="AssertionEntryExtensions.Should{T}(T, string?)" />.
/// </summary>
/// <remarks>
///     Failed assertions are reported as <see cref="AssertException" /> instances.
/// </remarks>
public readonly struct ShouldPolicy : IAssertionPolicy {
    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(string? expression, string message) {
        throw new AssertException(message, expression);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(string? expression, string message) {
        throw new AssertException(message, expression);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(string? expression, T actualValue, string message) {
        throw new AssertException(message, expression);
    }
}

/// <summary>
///     Failure policy used by <see cref="AssertionEntryExtensions.Guard{T}(T, string?)" />.
/// </summary>
/// <remarks>
///     Guard failures use the standard .NET argument exception hierarchy:
///     <see cref="ArgumentException" />, <see cref="ArgumentNullException" />,
///     or <see cref="ArgumentOutOfRangeException" />.
/// </remarks>
public readonly struct GuardPolicy : IAssertionPolicy {
    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(string? expression, string message) {
        throw new ArgumentException(message, expression);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(string? expression, string message) {
        throw new ArgumentNullException(expression, message);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(string? expression, T actualValue, string message) {
        throw new ArgumentOutOfRangeException(expression, actualValue, message);
    }
}

/// <summary>
///     Provides entry points for fluent assertions and argument guards.
/// </summary>
public static class AssertionEntryExtensions {
    /// <summary>
    ///     Starts a fluent runtime assertion for the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <param name="subject">The value to assert.</param>
    /// <param name="expression">
    ///     The source expression for <paramref name="subject" />, supplied automatically by the compiler.
    /// </param>
    /// <returns>
    ///     An assertion using <see cref="ShouldPolicy" />, which throws <see cref="AssertException" />
    ///     when a constraint fails.
    /// </returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, ShouldPolicy> Should<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? expression = null
    )
        => new(subject, expression);

    /// <summary>
    ///     Starts a fluent argument or precondition guard for the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the guarded argument.</typeparam>
    /// <param name="subject">The argument value to validate.</param>
    /// <param name="expression">
    ///     The source expression for <paramref name="subject" />, supplied automatically by the compiler
    ///     and used as the argument name when possible.
    /// </param>
    /// <returns>
    ///     An assertion using <see cref="GuardPolicy" />, which throws standard argument exceptions
    ///     when a constraint fails.
    /// </returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, GuardPolicy> Guard<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? expression = null
    )
        => new(subject, expression);
}

/// <summary>
///     Represents a failed runtime assertion.
/// </summary>
public sealed class AssertException : InvalidOperationException, IAssertionException {
    /// <summary>
    ///     Initializes a new instance of the <see cref="AssertException" /> class.
    /// </summary>
    /// <param name="message">The assertion failure message.</param>
    /// <param name="expression">The expression that produced the asserted value.</param>
    public AssertException(string message, string? expression = null)
        : base(CreateMessage(message, expression))
        => Expression = expression;

    /// <summary>
    ///     Gets the expression that produced the value for which the assertion failed.
    /// </summary>
    public string? Expression { get; }

    private static string CreateMessage(string message, string? expression)
        => expression is null
               ? message
               : $"{message}{Environment.NewLine}Expression: {expression}";
}

/// <summary>
///     Provides general-purpose fluent assertions.
/// </summary>
public static class CoreAssertions {
    /// <summary>
    ///     Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     Equality is evaluated using <see cref="EqualityComparer{T}.Default" />.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Be<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!EqualityComparer<T>.Default.Equals(assertion.Value, expected)) {
            TPolicy.Fail(assertion.Expression,
                         message ?? $"Expected '{expected}', but found '{assertion.Value}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is not equal to the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="unexpected">The value that must not equal the subject.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotBe<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T unexpected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (EqualityComparer<T>.Default.Equals(assertion.Value, unexpected)) {
            TPolicy.Fail(assertion.Expression,
                         message ?? $"Value must not be '{unexpected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the Boolean subject is <see langword="true" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<bool, TPolicy> BeTrue<TPolicy>(
        this Assertion<bool, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value)
            TPolicy.Fail(assertion.Expression, message ?? "Expected value to be true.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the Boolean subject is <see langword="false" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<bool, TPolicy> BeFalse<TPolicy>(
        this Assertion<bool, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value)
            TPolicy.Fail(assertion.Expression, message ?? "Expected value to be false.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that an externally evaluated condition is satisfied.
    /// </summary>
    /// <typeparam name="T">The type of the asserted subject.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="condition">The condition that must evaluate to <see langword="true" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the captured condition expression is used.
    /// </param>
    /// <param name="conditionExpression">
    ///     The source expression of <paramref name="condition" />, supplied automatically by the compiler.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Satisfy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        [DoesNotReturnIf(false)] bool condition,
        string? message = null,
        [CallerArgumentExpression("condition")] string? conditionExpression = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!condition) {
            TPolicy.Fail(assertion.Expression,
                         message ?? (conditionExpression is null
                                         ? "Condition was not satisfied."
                                         : $"Condition '{conditionExpression}' was not satisfied."));
        }

        return assertion;
    }
}

/// <summary>
///     Provides nullability assertions.
/// </summary>
public static class NullAssertions {
    /// <summary>
    ///     Asserts that a nullable reference is not <see langword="null" /> and refines the assertion
    ///     to its non-nullable reference type.
    /// </summary>
    /// <typeparam name="T">The non-nullable reference type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to non-null <typeparamref name="T" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotBeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : class
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Expression, message ?? "Value must not be null.");

        return new Assertion<T, TPolicy>(value!, assertion.Expression);
    }

    /// <summary>
    ///     Asserts that a nullable value type has a value and refines the assertion
    ///     to the underlying value type.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to non-null <typeparamref name="T" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotBeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : struct
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!value.HasValue)
            TPolicy.FailNull(assertion.Expression, message ?? "Value must have a value.");

        return new Assertion<T, TPolicy>(value.GetValueOrDefault(), assertion.Expression);
    }

    /// <summary>
    ///     Asserts that a nullable reference is <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T?, TPolicy> BeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : class
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value is not null)
            TPolicy.Fail(assertion.Expression, message ?? "Value must be null.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that a nullable value type has no value.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T?, TPolicy> BeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : struct
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.HasValue)
            TPolicy.Fail(assertion.Expression, message ?? "Value must be null.");

        return assertion;
    }
}

/// <summary>
///     Provides ordering and range assertions for comparable values.
/// </summary>
public static class ComparisonAssertions {
    /// <summary>
    ///     Asserts that the subject is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeGreaterThan<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value > expected)) {
            TPolicy.FailOutOfRange(assertion.Expression,
                                   assertion.Value,
                                   message ?? $"Value must be greater than '{expected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeGreaterThanOrEqualTo<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value >= expected)) {
            TPolicy.FailOutOfRange(assertion.Expression,
                                   assertion.Value,
                                   message ?? $"Value must be greater than or equal to '{expected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is less than the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeLessThan<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value < expected)) {
            TPolicy.FailOutOfRange(assertion.Expression,
                                   assertion.Value,
                                   message ?? $"Value must be less than '{expected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeLessThanOrEqualTo<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value <= expected)) {
            TPolicy.FailOutOfRange(assertion.Expression,
                                   assertion.Value,
                                   message ?? $"Value must be less than or equal to '{expected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The inclusive lower bound.</param>
    /// <param name="maximum">The inclusive upper bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeInRange<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T minimum,
        T maximum,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!(minimum <= value && value <= maximum)) {
            TPolicy.FailOutOfRange(assertion.Expression,
                                   value,
                                   message ?? $"Value must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }
}

/// <summary>
///     Provides assertions for IEEE 754 floating-point values.
/// </summary>
public static class FloatingPointAssertions {
    /// <summary>
    ///     Asserts that the subject differs from <paramref name="expected" /> by no more than
    ///     the specified absolute <paramref name="tolerance" />.
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="tolerance">The maximum permitted absolute difference from <paramref name="expected" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="tolerance" /> is negative or NaN.
    /// </exception>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> BeApproximately<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        T tolerance,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (T.IsNaN(tolerance) || tolerance < T.Zero) {
            throw new ArgumentOutOfRangeException(nameof(tolerance),
                                                  tolerance,
                                                  "Tolerance must be non-negative.");
        }

        var actual = assertion.Value;

        // Handles equal infinities as well.
        if (actual == expected)
            return assertion;

        var difference = T.Abs(actual - expected);

        if (T.IsNaN(difference) || difference > tolerance) {
            TPolicy.Fail(assertion.Expression,
                         message ?? $"Expected '{expected}' ± '{tolerance}', but found '{actual}'.");
        }

        return assertion;
    }
}

/// <summary>
///     Provides assertions specialized for strings.
/// </summary>
public static class StringAssertions {
    /// <summary>
    ///     Asserts that the string is neither <see langword="null" /> nor empty.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to a non-null <see cref="string" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotBeNullOrEmpty<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Expression, message ?? "String must not be null.");

        if (value.Length == 0)
            TPolicy.Fail(assertion.Expression, message ?? "String must not be empty.");

        return new Assertion<string, TPolicy>(value, assertion.Expression);
    }

    /// <summary>
    ///     Asserts that the string is neither <see langword="null" />, empty,
    ///     nor composed exclusively of white-space characters.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to a non-null <see cref="string" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotBeNullOrWhiteSpace<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Expression, message ?? "String must not be null.");

        if (string.IsNullOrWhiteSpace(value))
            TPolicy.Fail(assertion.Expression, message ?? "String must not be empty or whitespace.");

        return new Assertion<string, TPolicy>(value, assertion.Expression);
    }

    /// <summary>
    ///     Asserts that a non-null string is not empty.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotBeEmpty<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length == 0)
            TPolicy.Fail(assertion.Expression, message ?? "String must not be empty.");

        return assertion;
    }
}

/// <summary>
///     Provides assertions specialized for <see cref="Guid" /> values.
/// </summary>
public static class GuidAssertions {
    /// <summary>
    ///     Asserts that the GUID is not <see cref="Guid.Empty" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Guid, TPolicy> NotBeEmpty<TPolicy>(
        this Assertion<Guid, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value == Guid.Empty)
            TPolicy.Fail(assertion.Expression, message ?? "Guid must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that a nullable GUID has a value and is not <see cref="Guid.Empty" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to a non-null <see cref="Guid" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Guid, TPolicy> NotBeNullOrEmpty<TPolicy>(
        this Assertion<Guid?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!value.HasValue)
            TPolicy.FailNull(assertion.Expression, message ?? "Guid must not be null.");

        var guid = value.GetValueOrDefault();

        if (guid == Guid.Empty)
            TPolicy.Fail(assertion.Expression, message ?? "Guid must not be empty.");

        return new Assertion<Guid, TPolicy>(guid, assertion.Expression);
    }
}

/// <summary>
///     Provides assertions for collection sizes and array lengths.
/// </summary>
public static class CollectionAssertions {
    /// <summary>
    ///     Asserts that the collection contains at least one element.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> NotBeEmpty<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count == 0)
            TPolicy.Fail(assertion.Expression, message ?? "Collection must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the collection contains exactly the specified number of elements.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> HaveCount<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int expected,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual != expected) {
            TPolicy.Fail(assertion.Expression,
                         message ?? $"Expected count '{expected}', but found '{actual}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the array has exactly the specified length.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required array length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> HaveLength<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Length;

        if (actual != expected) {
            TPolicy.Fail(assertion.Expression,
                         message ?? $"Expected length '{expected}', but found '{actual}'.");
        }

        return assertion;
    }
}