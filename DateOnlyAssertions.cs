using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for calendar dates.
/// </summary>
/// <remarks>
///     Ordering checks (<c>After</c>, <c>Before</c>, <c>NotBefore</c>, <c>NotAfter</c>, <c>Range</c>) are covered by the
///     generic comparison assertions; only date-specific gaps live here.
/// </remarks>
public static class DateOnlyAssertions {
    /// <summary>
    ///     Asserts that the date is today.
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
    public static Assertion<DateOnly, TPolicy> Today<TPolicy>(
        this Assertion<DateOnly, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value != DateOnly.FromDateTime(DateTime.Today))
            TPolicy.Fail(assertion.Context, message ?? "Date must be today.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the date falls on a weekday (Monday through Friday).
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
    public static Assertion<DateOnly, TPolicy> Weekday<TPolicy>(
        this Assertion<DateOnly, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var dayOfWeek = assertion.Value.DayOfWeek;

        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            TPolicy.Fail(assertion.Context, message ?? "Date must be a weekday.");

        return assertion;
    }
}
