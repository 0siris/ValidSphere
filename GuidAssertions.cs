using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Assertions;

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
            TPolicy.Fail(assertion.Context, message ?? "Guid must not be empty.");

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
            TPolicy.FailNull(assertion.Context, message ?? "Guid must not be null.");

        var guid = value.GetValueOrDefault();

        if (guid == Guid.Empty)
            TPolicy.Fail(assertion.Context, message ?? "Guid must not be empty.");

        return new Assertion<Guid, TPolicy>(guid, assertion.Context);
    }
}