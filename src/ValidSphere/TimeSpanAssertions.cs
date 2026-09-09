using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions for <see cref="TimeSpan" /> values.
/// </summary>
public static class TimeSpanAssertions {
    /// <summary>
    ///     Asserts that the subject is positive (greater than <see cref="TimeSpan.Zero" />).
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
    public static Assertion<TimeSpan, TPolicy> Positive<TPolicy>(
        this Assertion<TimeSpan, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value > TimeSpan.Zero)) {
            assertion.FailOutOfRange(assertion.Value,
                                   message ?? "Value must be positive.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is non-negative (greater than or equal to <see cref="TimeSpan.Zero" />).
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
    public static Assertion<TimeSpan, TPolicy> NonNegative<TPolicy>(
        this Assertion<TimeSpan, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value >= TimeSpan.Zero)) {
            assertion.FailOutOfRange(assertion.Value,
                                   message ?? "Value must be non-negative.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is <see cref="TimeSpan.Zero" />.
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
    public static Assertion<TimeSpan, TPolicy> Zero<TPolicy>(
        this Assertion<TimeSpan, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value != TimeSpan.Zero) {
            assertion.Fail(message ?? "Value must be zero.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is a valid timeout: non-negative and at most <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted timeout.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TimeSpan, TPolicy> WithinTimeout<TPolicy>(
        this Assertion<TimeSpan, TPolicy> assertion,
        TimeSpan maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!(TimeSpan.Zero <= value && value <= maximum)) {
            assertion.FailOutOfRange(value,
                                   message ?? $"Timeout must be in range [{TimeSpan.Zero}, '{maximum}'].");
        }

        return assertion;
    }
}
