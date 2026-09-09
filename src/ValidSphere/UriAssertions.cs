using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for URIs.
/// </summary>
/// <remarks>
///     Strings switch into URI mode through <see cref="AsUri{TPolicy}(Assertion{string,TPolicy},UriKind,string?)" />,
///     and all URI checks run on <see cref="Uri" /> so malformed input fails at the refinement.
/// </remarks>
public static class UriAssertions {
    /// <summary>
    ///     Parses the asserted string into a <see cref="Uri" />, failing on invalid input.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="kind">The accepted URI kind. Defaults to <see cref="UriKind.Absolute" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The parsed <see cref="Uri" />.</returns>
    /// <remarks>
    ///     Terminal extractor: for chainable URI assertions use <c>url.Is().Uri()</c> instead.
    ///     Parsing checks syntax only, never reachability.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Uri AsUri<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        UriKind kind = UriKind.Absolute,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

        if (!System.Uri.TryCreate(value, kind, out var uri) || uri is null)
            assertion.Fail(message ?? $"String must be a valid '{kind}' URI.");

        return uri;
    }

    /// <summary>
    ///     Parses the asserted string into a <see cref="System.Uri" /> and returns a chainable URI assertion.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="kind">The accepted URI kind. Defaults to <see cref="UriKind.Absolute" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion over the parsed <see cref="System.Uri" />.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsUri()</c> instead.
    ///     Parsing checks syntax only, never reachability.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Uri, TPolicy> Uri<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        UriKind kind = UriKind.Absolute,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull("String must not be null.");

        if (!System.Uri.TryCreate(value, kind, out var uri) || uri is null)
            assertion.Fail(message ?? $"String must be a valid '{kind}' URI.");

        return assertion.Refine(uri);
    }

    /// <summary>
    ///     Asserts that the URI is absolute.
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
    public static Assertion<Uri, TPolicy> Absolute<TPolicy>(
        this Assertion<Uri, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Uri must not be null.");

        if (!value.IsAbsoluteUri)
            assertion.Fail(message ?? "Uri must be absolute.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the URI uses the given scheme.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="scheme">The expected scheme, exactly as <see cref="System.Uri.Scheme" /> reports it (e.g. "https").</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Uri, TPolicy> HaveScheme<TPolicy>(
        this Assertion<Uri, TPolicy> assertion,
        string scheme,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(scheme, nameof(scheme));

        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Uri must not be null.");

        if (!value.Scheme.Equals(scheme, comparison))
            assertion.Fail(message ?? $"Uri must have scheme '{scheme}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the URI targets the given host.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="host">The expected host.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Uri, TPolicy> HaveHost<TPolicy>(
        this Assertion<Uri, TPolicy> assertion,
        string host,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(host, nameof(host));

        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Uri must not be null.");

        if (!value.Host.Equals(host, comparison))
            assertion.Fail(message ?? $"Uri must have host '{host}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the URI uses the given port.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="port">The expected port. Scheme default ports count (https without a port is 443).</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<Uri, TPolicy> HavePort<TPolicy>(
        this Assertion<Uri, TPolicy> assertion,
        int port,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Uri must not be null.");

        if (value.Port != port)
            assertion.Fail(message ?? $"Uri must have port '{port}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the URI is a loopback URI.
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
    public static Assertion<Uri, TPolicy> Loopback<TPolicy>(
        this Assertion<Uri, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Uri must not be null.");

        if (!value.IsLoopback)
            assertion.Fail(message ?? "Uri must be a loopback URI.");

        return assertion;
    }
}
