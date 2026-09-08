using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for date-time offsets.
/// </summary>
/// <remarks>
///     Ordering checks (<c>After</c>, <c>Before</c>, <c>NotBefore</c>, <c>NotAfter</c>, <c>Range</c>) are covered by the
///     generic comparison assertions; only offset-specific gaps live here.
/// </remarks>
public static class DateTimeOffsetAssertions {
    /// <summary>
    ///     Asserts that the offset is UTC (zero offset).
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
    public static Assertion<DateTimeOffset, TPolicy> Utc<TPolicy>(
        this Assertion<DateTimeOffset, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Offset != TimeSpan.Zero)
            TPolicy.Fail(assertion.Context, message ?? "DateTimeOffset must be UTC.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the offset equals the given UTC offset.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="offset">The expected UTC offset.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DateTimeOffset, TPolicy> HaveOffset<TPolicy>(
        this Assertion<DateTimeOffset, TPolicy> assertion,
        TimeSpan offset,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Offset != offset)
            TPolicy.Fail(assertion.Context, message ?? $"DateTimeOffset must have offset '{offset}'.");

        return assertion;
    }
}
