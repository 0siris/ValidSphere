using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

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
    public static Assertion<TCollection, TPolicy> NotEmpty<TCollection, TPolicy>(
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
    ///     Asserts that the collection contains at least one element, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> NotEmpty<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        Func<AssertionContext, string> messageFactory
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count == 0)
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

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
    public static Assertion<TCollection, TPolicy> Count<TCollection, TPolicy>(
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
    ///     Asserts that the collection contains exactly the specified number of elements, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> Count<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int expected,
        Func<AssertionContext, string> messageFactory
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual != expected) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
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
    public static Assertion<T[], TPolicy> Length<T, TPolicy>(
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
    /// <summary>
    ///     Asserts that the array has exactly the specified length, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> Length<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int expected,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Length;

        if (actual != expected) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the collection contains at least the specified number of elements.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> MinCount<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int minimum,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count < minimum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Count,
                                   message ?? $"Collection must contain at least '{minimum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the collection contains at most the specified number of elements.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> MaxCount<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int maximum,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Count,
                                   message ?? $"Collection must contain at most '{maximum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the number of elements lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted number of elements.</param>
    /// <param name="maximum">The maximum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> CountInRange<TCollection, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        int minimum,
        int maximum,
        string? message = null
    )
        where TCollection : ICollection
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual < minimum || actual > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   actual,
                                   message ?? $"Collection count must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the read-only collection contains at least one element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyCollection<T>, TPolicy> NotEmpty<T, TPolicy>(
        this Assertion<IReadOnlyCollection<T>, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count == 0)
            TPolicy.Fail(assertion.Context, message ?? "Collection must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the read-only collection contains exactly the specified number of elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
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
    public static Assertion<IReadOnlyCollection<T>, TPolicy> Count<T, TPolicy>(
        this Assertion<IReadOnlyCollection<T>, TPolicy> assertion,
        int expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual != expected) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"Expected count '{expected}', but found '{actual}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the read-only collection contains at least the specified number of elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyCollection<T>, TPolicy> MinCount<T, TPolicy>(
        this Assertion<IReadOnlyCollection<T>, TPolicy> assertion,
        int minimum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count < minimum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Count,
                                   message ?? $"Collection must contain at least '{minimum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the read-only collection contains at most the specified number of elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyCollection<T>, TPolicy> MaxCount<T, TPolicy>(
        this Assertion<IReadOnlyCollection<T>, TPolicy> assertion,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Count > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Count,
                                   message ?? $"Collection must contain at most '{maximum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the number of elements in the read-only collection lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted number of elements.</param>
    /// <param name="maximum">The maximum permitted number of elements.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyCollection<T>, TPolicy> CountInRange<T, TPolicy>(
        this Assertion<IReadOnlyCollection<T>, TPolicy> assertion,
        int minimum,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Count;

        if (actual < minimum || actual > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   actual,
                                   message ?? $"Collection count must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the array length is at least the specified minimum.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted array length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> MinLength<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int minimum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length < minimum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Length,
                                   message ?? $"Array must have at least '{minimum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the array length is at most the specified maximum.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted array length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> MaxLength<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   assertion.Value.Length,
                                   message ?? $"Array must have at most '{maximum}' elements.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the array length lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted array length.</param>
    /// <param name="maximum">The maximum permitted array length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T[], TPolicy> LengthInRange<T, TPolicy>(
        this Assertion<T[], TPolicy> assertion,
        int minimum,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Length;

        if (actual < minimum || actual > maximum) {
            TPolicy.FailOutOfRange(assertion.Context,
                                   actual,
                                   message ?? $"Array length must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the collection contains the specified element.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TItem">The element type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required element.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     Iteration stops at the first match and allocates nothing.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TCollection, TPolicy> Contains<TCollection, TItem, TPolicy>(
        this Assertion<TCollection, TPolicy> assertion,
        TItem expected,
        string? message = null
    )
        where TCollection : ICollection<TItem>
        where TPolicy : struct, IAssertionPolicy {
        var comparer = EqualityComparer<TItem>.Default;

        foreach (var item in assertion.Value) {
            if (comparer.Equals(item, expected))
                return assertion;
        }

        TPolicy.Fail(assertion.Context, message ?? $"Collection must contain '{expected}'.");

        return assertion;
    }
}
