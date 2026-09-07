using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Assertions;

/// <summary>
///     Provides nullability assertions.
/// </summary>
public static class NullAssertions {
    /// <summary>
    ///     Asserts that a nullable reference is not <see langword="null" /> and refines the assertion
    ///     to its non-nullable reference type.
    /// </summary>
    /// <typeparam name="T">The non-nullable reference type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to non-null <typeparamref name="T" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotBeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : class
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, message ?? "Value must not be null.");

        return new Assertion<T, TPolicy>(value!, assertion.Context);
    }

    /// <summary>
    ///     Asserts that a nullable value type has a value and refines the assertion
    ///     to the underlying value type.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion refined to non-null <typeparamref name="T" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotBeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : struct
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (!value.HasValue)
            TPolicy.FailNull(assertion.Context, message ?? "Value must have a value.");

        return new Assertion<T, TPolicy>(value.GetValueOrDefault(), assertion.Context);
    }

    /// <summary>
    ///     Asserts that a nullable reference is <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T?, TPolicy> BeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : class
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value is not null)
            TPolicy.Fail(assertion.Context, message ?? "Value must be null.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that a nullable value type has no value.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T?, TPolicy> BeNull<T, TPolicy>(
        this Assertion<T?, TPolicy> assertion,
        string? message = null
    )
        where T : struct
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.HasValue)
            TPolicy.Fail(assertion.Context, message ?? "Value must be null.");

        return assertion;
    }
}