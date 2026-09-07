using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Assertions;

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
            TPolicy.Fail(assertion.Context,
                         message ?? $"Expected '{expected}' ± '{tolerance}', but found '{actual}'.");
        }

        return assertion;
    }
}