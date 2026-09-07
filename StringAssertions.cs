using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Assertions;

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
    public static Assertion<string, TPolicy> NotNullOrEmpty<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, message ?? "String must not be null.");

        if (value.Length == 0)
            TPolicy.Fail(assertion.Context, message ?? "String must not be empty.");

        return new Assertion<string, TPolicy>(value, assertion.Context);
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
    public static Assertion<string, TPolicy> NotNullOrWhiteSpace<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, message ?? "String must not be null.");

        if (string.IsNullOrWhiteSpace(value))
            TPolicy.Fail(assertion.Context, message ?? "String must not be empty or whitespace.");

        return new Assertion<string, TPolicy>(value, assertion.Context);
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
    public static Assertion<string, TPolicy> NotEmpty<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length == 0)
            TPolicy.Fail(assertion.Context, message ?? "String must not be empty.");

        return assertion;
    }
}