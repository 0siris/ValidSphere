using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

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
    public static Assertion<T, TPolicy> Approx<T, TPolicy>(
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
            assertion.Fail(message ?? $"Expected '{expected}' ± '{tolerance}', but found '{actual}'.");
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts approximate equality, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Approx<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        T tolerance,
        Func<AssertionContext, string> messageFactory
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
            assertion.Fail(messageFactory(assertion.Context));
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the subject is not <see langword="NaN" />.
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotNaN<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (T.IsNaN(assertion.Value)) {
            assertion.Fail(message ?? "Value must not be NaN.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is finite (neither <see langword="NaN" /> nor infinite).
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Finite<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsFinite(assertion.Value)) {
            assertion.Fail(message ?? "Value must be finite.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is infinite (positive or negative infinity).
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Infinite<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsInfinity(assertion.Value)) {
            assertion.Fail(message ?? "Value must be infinite.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is positive infinity.
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> PositiveInfinity<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsPositiveInfinity(assertion.Value)) {
            assertion.Fail(message ?? "Value must be positive infinity.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is negative infinity.
    /// </summary>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NegativeInfinity<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IFloatingPointIeee754<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsNegativeInfinity(assertion.Value)) {
            assertion.Fail(message ?? "Value must be negative infinity.");
        }

        return assertion;
    }
}
