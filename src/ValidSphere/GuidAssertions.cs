using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for <see cref="Guid" /> values.
/// </summary>
public static class GuidAssertions {
    /// <summary>
    ///     Asserts that the GUID is not <see cref="System.Guid.Empty" />.
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
    public static Assertion<Guid, TPolicy> NotEmpty<TPolicy>(
        this Assertion<Guid, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value == System.Guid.Empty)
            assertion.Fail(message ?? "Guid must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that a nullable GUID has a value and is not <see cref="System.Guid.Empty" />.
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
    public static Assertion<Guid, TPolicy> NotNullOrEmpty<TPolicy>(
        this Assertion<Guid?, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!value.HasValue)
            assertion.FailNull(message ?? "Guid must not be null.");

        var guid = value.GetValueOrDefault();

        if (guid == System.Guid.Empty)
            assertion.Fail(message ?? "Guid must not be empty.");

        return assertion.Refine(guid);
    }

    /// <summary>
    ///     Parses the asserted string into a <see cref="Guid" />, failing on invalid input.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The parsed <see cref="Guid" />.</returns>
    /// <remarks>
    ///     Terminal extractor: for chainable GUID assertions use <c>trackingId.Is().Guid()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid AsGuid<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

        if (!System.Guid.TryParse(value, out var guid))
            assertion.Fail(message ?? "String must be a valid GUID.");

        return guid;
    }

    /// <summary>
    ///     Parses the asserted string into a <see cref="System.Guid" /> and returns a chainable GUID assertion.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion over the parsed <see cref="System.Guid" />.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsGuid()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Guid, TPolicy> Guid<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

        if (!System.Guid.TryParse(value, out var guid))
            assertion.Fail(message ?? "String must be a valid GUID.");

        return assertion.Refine(guid);
    }
}