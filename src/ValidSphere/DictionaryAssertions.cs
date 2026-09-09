using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions for dictionaries.
/// </summary>
/// <remarks>
///     Overloads are declared per dictionary type (<see cref="Dictionary{TKey, TValue}" />,
///     <see cref="IDictionary{TKey, TValue}" />, <see cref="IReadOnlyDictionary{TKey, TValue}" />) so the
///     key and value types infer from the assertion subject. A single generic
///     <c>TDict : IReadOnlyDictionary&lt;TKey, TValue&gt;</c> overload would force callers to specify
///     <c>TKey</c>/<c>TValue</c> explicitly because type inference ignores constraints.
/// </remarks>
public static class DictionaryAssertions {
    /// <summary>
    ///     Asserts that the dictionary contains the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Dictionary<TKey, TValue>, TPolicy> ContainsKey<TKey, TValue, TPolicy>(
        this Assertion<Dictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TKey : notnull
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must contain key '{key}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the dictionary contains the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IDictionary<TKey, TValue>, TPolicy> ContainsKey<TKey, TValue, TPolicy>(
        this Assertion<IDictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must contain key '{key}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the dictionary contains the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyDictionary<TKey, TValue>, TPolicy> ContainsKey<TKey, TValue, TPolicy>(
        this Assertion<IReadOnlyDictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must contain key '{key}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the dictionary does not contain the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Dictionary<TKey, TValue>, TPolicy> NotContainsKey<TKey, TValue, TPolicy>(
        this Assertion<Dictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TKey : notnull
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must not contain key '{key}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the dictionary does not contain the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IDictionary<TKey, TValue>, TPolicy> NotContainsKey<TKey, TValue, TPolicy>(
        this Assertion<IDictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must not contain key '{key}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the dictionary does not contain the specified key.
    /// </summary>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IReadOnlyDictionary<TKey, TValue>, TPolicy> NotContainsKey<TKey, TValue, TPolicy>(
        this Assertion<IReadOnlyDictionary<TKey, TValue>, TPolicy> assertion,
        TKey key,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.ContainsKey(key)) {
            assertion.Fail(message ?? $"Dictionary must not contain key '{key}'.");
        }

        return assertion;
    }
}
