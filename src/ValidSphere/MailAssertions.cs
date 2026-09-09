using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for mail addresses.
/// </summary>
/// <remarks>
///     Strings switch into mail mode through <see cref="AsMailAddress{TPolicy}(Assertion{string,TPolicy},string?)" />,
///     and all mail checks run on <see cref="MailAddress" /> so malformed input fails at the refinement.
/// </remarks>
public static class MailAssertions {
    /// <summary>
    ///     Parses the asserted string into a <see cref="MailAddress" />, failing on invalid input.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The parsed <see cref="MailAddress" />.</returns>
    /// <remarks>
    ///     Terminal extractor: for chainable mail assertions use <c>mail.Is().MailAddress()</c> instead.
    ///     Parsing checks syntax only, never reachability.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MailAddress AsMailAddress<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, "String must not be null.");

#if NETSTANDARD2_1
        MailAddress? address = null;
        try {
            address = new System.Net.Mail.MailAddress(value);
        }
        catch (FormatException) { }
        catch (ArgumentException) { }
        if (address is null)
#else
        if (!System.Net.Mail.MailAddress.TryCreate(value, out var address) || address is null)
#endif
            TPolicy.Fail(assertion.Context, message ?? "String must be a valid mail address.");

        return address;
    }

    /// <summary>
    ///     Parses the asserted string into a <see cref="System.Net.Mail.MailAddress" /> and returns a chainable mail assertion.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion over the parsed <see cref="System.Net.Mail.MailAddress" />.</returns>
    /// <remarks>
    ///     Chain entry: for the raw value use <c>AsMailAddress()</c> instead.
    ///     Parsing checks syntax only, never reachability.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<MailAddress, TPolicy> MailAddress<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, "String must not be null.");

#if NETSTANDARD2_1
        MailAddress? address = null;
        try {
            address = new System.Net.Mail.MailAddress(value);
        }
        catch (FormatException) { }
        catch (ArgumentException) { }
        if (address is null)
#else
        if (!System.Net.Mail.MailAddress.TryCreate(value, out var address) || address is null)
#endif
            TPolicy.Fail(assertion.Context, message ?? "String must be a valid mail address.");

        return new Assertion<MailAddress, TPolicy>(address, assertion.Context);
    }

    /// <summary>
    ///     Asserts that the mail address targets the given host.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="host">The expected host (the part after '@').</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<MailAddress, TPolicy> HaveHost<TPolicy>(
        this Assertion<MailAddress, TPolicy> assertion,
        string host,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(host, nameof(host));

        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, message ?? "Mail address must not be null.");

        if (!value.Host.Equals(host, comparison))
            TPolicy.Fail(assertion.Context, message ?? $"Mail address must have host '{host}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the mail address has the given user part.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="user">The expected user (the part before '@').</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<MailAddress, TPolicy> HaveUser<TPolicy>(
        this Assertion<MailAddress, TPolicy> assertion,
        string user,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(user, nameof(user));

        var value = assertion.Value;

        if (value is null)
            TPolicy.FailNull(assertion.Context, message ?? "Mail address must not be null.");

        if (!value.User.Equals(user, comparison))
            TPolicy.Fail(assertion.Context, message ?? $"Mail address must have user '{user}'.");

        return assertion;
    }
}
