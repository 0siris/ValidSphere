using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions for <see cref="DateTime" /> values.
/// </summary>
/// <remarks>
///     Reference clocks are always supplied by the caller (for example <c>DateTime.UtcNow</c>);
///     assertions never read the clock themselves.
/// </remarks>
public static class DateTimeAssertions {
    /// <summary>
    ///     Asserts that the subject has the specified <see cref="DateTimeKind" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required kind.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTime, TPolicy> Kind<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        DateTimeKind expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Kind != expected) {
            assertion.Fail(message ?? $"DateTime must have kind '{expected}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is in UTC (<see cref="DateTimeKind.Utc" />).
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
    public static Assertion<DateTime, TPolicy> Utc<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Kind != DateTimeKind.Utc) {
            assertion.Fail(message ?? "DateTime must be UTC.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is strictly after the specified bound.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="exclusiveLowerBound">The exclusive lower bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTime, TPolicy> After<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        DateTime exclusiveLowerBound,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value > exclusiveLowerBound)) {
            assertion.FailOutOfRange(assertion.Value,
                                     message ?? $"Value must be after '{exclusiveLowerBound}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is not before the specified bound.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="lowerBound">The lower bound (inclusive).</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTime, TPolicy> NotBefore<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        DateTime lowerBound,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value >= lowerBound)) {
            assertion.FailOutOfRange(assertion.Value,
                                     message ?? $"Value must not be before '{lowerBound}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is strictly before the specified bound.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="exclusiveUpperBound">The exclusive upper bound.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTime, TPolicy> Before<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        DateTime exclusiveUpperBound,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value < exclusiveUpperBound)) {
            assertion.FailOutOfRange(assertion.Value,
                                     message ?? $"Value must be before '{exclusiveUpperBound}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is not after the specified bound.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="upperBound">The upper bound (inclusive).</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTime, TPolicy> NotAfter<TPolicy>(
        this Assertion<DateTime, TPolicy> assertion,
        DateTime upperBound,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value <= upperBound)) {
            assertion.FailOutOfRange(assertion.Value,
                                     message ?? $"Value must not be after '{upperBound}'.");
        }

        return assertion;
    }
}