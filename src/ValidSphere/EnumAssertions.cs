using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions for enum values.
/// </summary>
/// <remarks>
///     Parsing entries are policy-fixed single-type-parameter methods: C# forbids partial
///     type-argument lists, so a `TPolicy`-generic `Enum&lt;TEnum, TPolicy&gt;` could never be
///     called as `.Enum&lt;Color&gt;()`. The Is-side keeps the chain shape
///     (`value.Is().Enum&lt;Color&gt;()`), the Guard-side uses `GuardEnum`/`AsGuardEnum` names.
/// </remarks>
public static class EnumAssertions {
    /// <summary>
    ///     Asserts that the enum value is defined.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, TPolicy> Defined<TEnum, TPolicy>(
        this Assertion<TEnum, TPolicy> assertion,
        string? message = null
    )
        where TEnum : struct, Enum
        where TPolicy : struct, IAssertionPolicy {
#if NETSTANDARD2_1
        if (!System.Enum.IsDefined(typeof(TEnum), assertion.Value))
#else
        if (!System.Enum.IsDefined(assertion.Value))
#endif
            assertion.Fail(message ?? "Enum value must be defined");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the enum value is defined, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, TPolicy> Defined<TEnum, TPolicy>(
        this Assertion<TEnum, TPolicy> assertion,
        Func<AssertionContext, string> messageFactory
    )
        where TEnum : struct, Enum
        where TPolicy : struct, IAssertionPolicy {
#if NETSTANDARD2_1
        if (!System.Enum.IsDefined(typeof(TEnum), assertion.Value))
#else
        if (!System.Enum.IsDefined(assertion.Value))
#endif
            assertion.Fail(messageFactory(assertion.Context));

        return assertion;
    }

    /// <summary>
    ///     Asserts that the enum value includes the given flag.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="flag">The flag that must be set. May combine flags.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     A plain bit test; the enum is not required to carry <see cref="FlagsAttribute" />.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, TPolicy> Flagged<TEnum, TPolicy>(
        this Assertion<TEnum, TPolicy> assertion,
        TEnum flag,
        string? message = null
    )
        where TEnum : struct, Enum
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value.HasFlag(flag))
            assertion.Fail(message ?? $"Enum value must have flag '{flag}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the enum value includes the given flag, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, TPolicy> Flagged<TEnum, TPolicy>(
        this Assertion<TEnum, TPolicy> assertion,
        TEnum flag,
        Func<AssertionContext, string> messageFactory
    )
        where TEnum : struct, Enum
        where TPolicy : struct, IAssertionPolicy {
        if (!assertion.Value.HasFlag(flag))
            assertion.Fail(messageFactory(assertion.Context));

        return assertion;
    }

    /// <summary>
    ///     Parses the asserted string into the enum and returns a chainable enum assertion.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="ignoreCase">
    ///     <see langword="true" /> to ignore case when parsing the enum name. Defaults to <see langword="false" />.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion over the parsed enum value.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsEnum()</c> instead.
    ///     Parsing checks the name shape only; chain <c>Defined()</c> for a strict name check.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, IsPolicy> Enum<TEnum>(
        this Assertion<string, IsPolicy> assertion,
        bool ignoreCase = false,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

#if NETSTANDARD2_1
        if (!System.Enum.TryParse(typeof(TEnum), value, ignoreCase, out var parsedRaw) || parsedRaw is null)
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        var parsed = (TEnum)parsedRaw;
#else
        if (!System.Enum.TryParse<TEnum>(value, ignoreCase, out var parsed))
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");
#endif

        return assertion.Refine(parsed);
    }

    /// <summary>
    ///     Parses the asserted string into the enum and returns a chainable enum guard.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="ignoreCase">
    ///     <see langword="true" /> to ignore case when parsing the enum name. Defaults to <see langword="false" />.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default guard message is used.
    /// </param>
    /// <returns>A guard assertion over the parsed enum value.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsGuardEnum()</c> instead.
    ///     Parsing checks the name shape only; chain <c>Defined()</c> for a strict name check.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, GuardPolicy> GuardEnum<TEnum>(
        this Assertion<string, GuardPolicy> assertion,
        bool ignoreCase = false,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

#if NETSTANDARD2_1
        if (!System.Enum.TryParse(typeof(TEnum), value, ignoreCase, out var parsedRaw) || parsedRaw is null)
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        var parsed = (TEnum)parsedRaw;
#else
        if (!System.Enum.TryParse<TEnum>(value, ignoreCase, out var parsed))
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");
#endif

        return assertion.Refine(parsed);
    }

    /// <summary>
    ///     Parses the asserted string into the enum, failing on invalid input.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="ignoreCase">
    ///     <see langword="true" /> to ignore case when parsing the enum name. Defaults to <see langword="false" />.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The parsed enum value.</returns>
    /// <remarks>
    ///     Terminal extractor: for a chainable enum assertion use <c>Enum()</c> instead.
    ///     Parsing checks the name shape only; chain <c>Defined()</c> for a strict name check.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum AsEnum<TEnum>(
        this Assertion<string, IsPolicy> assertion,
        bool ignoreCase = false,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

#if NETSTANDARD2_1
        if (!System.Enum.TryParse(typeof(TEnum), value, ignoreCase, out var parsedRaw) || parsedRaw is null)
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        return (TEnum)parsedRaw;
#else
        if (!System.Enum.TryParse<TEnum>(value, ignoreCase, out var parsed))
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        return parsed;
#endif
    }

    /// <summary>
    ///     Parses the asserted string into the enum, failing on invalid input.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="ignoreCase">
    ///     <see langword="true" /> to ignore case when parsing the enum name. Defaults to <see langword="false" />.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default guard message is used.
    /// </param>
    /// <returns>The parsed enum value.</returns>
    /// <remarks>
    ///     Terminal extractor: for a chainable enum guard use <c>GuardEnum()</c> instead.
    ///     Parsing checks the name shape only; chain <c>Defined()</c> for a strict name check.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum AsGuardEnum<TEnum>(
        this Assertion<string, GuardPolicy> assertion,
        bool ignoreCase = false,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

#if NETSTANDARD2_1
        if (!System.Enum.TryParse(typeof(TEnum), value, ignoreCase, out var parsedRaw) || parsedRaw is null)
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        return (TEnum)parsedRaw;
#else
        if (!System.Enum.TryParse<TEnum>(value, ignoreCase, out var parsed))
            assertion.Fail(message ?? $"String must be a valid '{typeof(TEnum).Name}' enum value.");

        return parsed;
#endif
    }

    /// <summary>
    ///     Converts the asserted integer into the enum, requiring a defined value, and returns a chainable enum assertion.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion over the converted enum value.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsEnum()</c> instead.
    ///     Unlike the string pair, the integer pair always requires a defined value, since a raw cast never fails.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, IsPolicy> Enum<TEnum>(
        this Assertion<int, IsPolicy> assertion,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (!System.Enum.IsDefined(typeof(TEnum), value))
            assertion.Fail(message ?? $"Value must be a defined '{typeof(TEnum).Name}' enum value.");

        var parsed = (TEnum)System.Enum.ToObject(typeof(TEnum), value);

        return assertion.Refine(parsed);
    }

    /// <summary>
    ///     Converts the asserted integer into the enum, requiring a defined value, and returns a chainable enum guard.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default guard message is used.
    /// </param>
    /// <returns>A guard assertion over the converted enum value.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsGuardEnum()</c> instead.
    ///     Unlike the string pair, the integer pair always requires a defined value, since a raw cast never fails.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<TEnum, GuardPolicy> GuardEnum<TEnum>(
        this Assertion<int, GuardPolicy> assertion,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (!System.Enum.IsDefined(typeof(TEnum), value))
            assertion.Fail(message ?? $"Value must be a defined '{typeof(TEnum).Name}' enum value.");

        var parsed = (TEnum)System.Enum.ToObject(typeof(TEnum), value);

        return assertion.Refine(parsed);
    }

    /// <summary>
    ///     Converts the asserted integer into the enum, requiring a defined value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The converted enum value.</returns>
    /// <remarks>
    ///     Terminal extractor: for a chainable enum assertion use <c>Enum()</c> instead.
    ///     Unlike the string pair, the integer pair always requires a defined value, since a raw cast never fails.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum AsEnum<TEnum>(
        this Assertion<int, IsPolicy> assertion,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (!System.Enum.IsDefined(typeof(TEnum), value))
            assertion.Fail(message ?? $"Value must be a defined '{typeof(TEnum).Name}' enum value.");

        return (TEnum)System.Enum.ToObject(typeof(TEnum), value);
    }

    /// <summary>
    ///     Converts the asserted integer into the enum, requiring a defined value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default guard message is used.
    /// </param>
    /// <returns>The converted enum value.</returns>
    /// <remarks>
    ///     Terminal extractor: for a chainable enum guard use <c>GuardEnum()</c> instead.
    ///     Unlike the string pair, the integer pair always requires a defined value, since a raw cast never fails.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum AsGuardEnum<TEnum>(
        this Assertion<int, GuardPolicy> assertion,
        string? message = null
    )
        where TEnum : struct, Enum {
        var value = assertion.Value;

        if (!System.Enum.IsDefined(typeof(TEnum), value))
            assertion.Fail(message ?? $"Value must be a defined '{typeof(TEnum).Name}' enum value.");

        return (TEnum)System.Enum.ToObject(typeof(TEnum), value);
    }
}