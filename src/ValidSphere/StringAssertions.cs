using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ValidSphere;

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
    ///     Asserts that the string is neither <see langword="null" /> nor empty, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotNullOrEmpty<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, messageFactory(assertion.Context));

        if (value.Length == 0)
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

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
    ///     Asserts that the string is neither <see langword="null" />, empty, nor whitespace, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotNullOrWhiteSpace<TPolicy>(
        this Assertion<string?, TPolicy> assertion,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, messageFactory(assertion.Context));

        if (string.IsNullOrWhiteSpace(value))
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

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
    /// <summary>
    ///     Asserts that a non-null string is not empty, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> NotEmpty<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length == 0)
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

        return assertion;
    }
    /// <summary>
    ///     Asserts that the string has exactly the specified length.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required string length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> Length<TPolicy>(
        this Assertion<string, TPolicy> assertion,
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
    ///     Asserts that the string is at least the specified length.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted string length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> MinLength<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        int minimum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length < minimum) {
            TPolicy.Fail(assertion.Context, message ?? $"String length must be at least '{minimum}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string is at most the specified length.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted string length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> MaxLength<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (assertion.Value.Length > maximum) {
            TPolicy.Fail(assertion.Context, message ?? $"String length must be at most '{maximum}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string length lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted string length.</param>
    /// <param name="maximum">The maximum permitted string length.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> LengthInRange<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        int minimum,
        int maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var actual = assertion.Value.Length;

        if (actual < minimum || actual > maximum) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"String length must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string contains the specified substring.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="substring">The required substring.</param>
    /// <param name="comparison">The comparison rules.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> Contains<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string substring,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(substring, nameof(substring));

        if (!assertion.Value.Contains(substring, comparison)) {
            TPolicy.Fail(assertion.Context, message ?? $"String must contain '{substring}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string starts with the specified prefix.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="prefix">The required prefix.</param>
    /// <param name="comparison">The comparison rules.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> StartsWith<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string prefix,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(prefix, nameof(prefix));

        if (!assertion.Value.StartsWith(prefix, comparison)) {
            TPolicy.Fail(assertion.Context, message ?? $"String must start with '{prefix}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string ends with the specified suffix.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="suffix">The required suffix.</param>
    /// <param name="comparison">The comparison rules.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> EndsWith<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string suffix,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(suffix, nameof(suffix));

        if (!assertion.Value.EndsWith(suffix, comparison)) {
            TPolicy.Fail(assertion.Context, message ?? $"String must end with '{suffix}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the string matches the specified regular expression.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="regex">The required pattern. Reuse the instance (for example via GeneratedRegex); the pattern is never compiled here.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<string, TPolicy> Matches<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        Regex regex,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(regex, nameof(regex));

        if (!regex.IsMatch(assertion.Value)) {
            TPolicy.Fail(assertion.Context, message ?? "String must match the required pattern.");
        }

        return assertion;
    }
}
