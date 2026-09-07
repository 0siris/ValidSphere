using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Assertions;

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
    public static Assertion<T, TPolicy> Greater<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value > expected)) {
            TPolicy.FailOutOfRange(assertion.Context,
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
    public static Assertion<T, TPolicy> GreaterEq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value >= expected)) {
            TPolicy.FailOutOfRange(assertion.Context,
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
    public static Assertion<T, TPolicy> Less<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value < expected)) {
            TPolicy.FailOutOfRange(assertion.Context,
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
    public static Assertion<T, TPolicy> LessEq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value <= expected)) {
            TPolicy.FailOutOfRange(assertion.Context,
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
    public static Assertion<T, TPolicy> Range<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T minimum,
        T maximum,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!(minimum <= value && value <= maximum)) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   value,
                                   message ?? $"Value must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }
}