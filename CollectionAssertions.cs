using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Assertions;

/// <summary>
///     Provides assertions for collection sizes and array lengths.
/// </summary>
public static class CollectionAssertions {
    /// <summary>
    ///     Asserts that the collection contains at least one element.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> NotBeEmpty<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count == 0)
            TPolicy.Fail(assertion.Context, message ?? "Collection must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the collection contains exactly the specified number of elements.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> HaveCount<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int expected,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual != expected) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"Expected count '{expected}', but found '{actual}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the array has exactly the specified length.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required array length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> HaveLength<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Length;

        if (actual != expected) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"Expected length '{expected}', but found '{actual}'.");
        }

        return assertion;
    }
}