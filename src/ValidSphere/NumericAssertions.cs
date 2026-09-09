using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides domain assertions for numeric values.
/// </summary>
public static class NumericAssertions {
    /// <summary>
    ///     Asserts that the subject is positive (greater than zero).
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Positive<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value > T.Zero)) {
            assertion.FailOutOfRange(assertion.Value,
                                   message ?? "Value must be positive.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is non-negative (greater than or equal to zero).
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NonNegative<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value >= T.Zero)) {
            assertion.FailOutOfRange(assertion.Value,
                                   message ?? "Value must be non-negative.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is negative (less than zero).
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Negative<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IComparisonOperators<T, T, bool>, INumberBase<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value < T.Zero)) {
            assertion.FailOutOfRange(assertion.Value,
                                   message ?? "Value must be negative.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is zero.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Zero<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IEqualityOperators<T, T, bool>, INumberBase<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!(assertion.Value == T.Zero)) {
            assertion.Fail(message ?? "Value must be zero.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is not zero.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NonZero<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IEqualityOperators<T, T, bool>, INumberBase<T>
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value == T.Zero) {
            assertion.Fail(message ?? "Value must be non-zero.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is even.
    /// </summary>
    /// <typeparam name="T">The integer type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Even<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IBinaryInteger<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsEvenInteger(assertion.Value)) {
            assertion.Fail(message ?? "Value must be even.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is odd.
    /// </summary>
    /// <typeparam name="T">The integer type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Odd<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        string? message = null
    )
        where T : IBinaryInteger<T>
        where TPolicy : struct, IAssertionPolicy {
        if (!T.IsOddInteger(assertion.Value)) {
            assertion.Fail(message ?? "Value must be odd.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the subject is divisible by <paramref name="divisor" />.
    /// </summary>
    /// <typeparam name="T">The integer type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="divisor">The required divisor.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     A zero <paramref name="divisor" /> is a programming error and surfaces as the BCL
    ///     <see cref="DivideByZeroException" /> from the remainder operation.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> DivisibleBy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T divisor,
        string? message = null
    )
        where T : IBinaryInteger<T>
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value % divisor != T.Zero) {
            assertion.Fail(message ?? $"Value must be divisible by '{divisor}'.");
        }

        return assertion;
    }
}
