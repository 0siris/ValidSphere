using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides assertions specialized for IP addresses and endpoints.
/// </summary>
public static class IpAssertions {
    /// <summary>
    ///     Asserts that the IP address is a loopback address.
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
    public static Assertion<IPAddress, TPolicy> Loopback<TPolicy>(
        this Assertion<IPAddress, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "IP address must not be null.");

        if (!IPAddress.IsLoopback(value))
            assertion.Fail(message ?? "IP address must be a loopback address.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the IP address is an IPv4 address.
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
    public static Assertion<IPAddress, TPolicy> IPv4<TPolicy>(
        this Assertion<IPAddress, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "IP address must not be null.");

        if (value.AddressFamily != AddressFamily.InterNetwork)
            assertion.Fail(message ?? "IP address must be IPv4.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the IP address is an IPv6 address.
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
    public static Assertion<IPAddress, TPolicy> IPv6<TPolicy>(
        this Assertion<IPAddress, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "IP address must not be null.");

        if (value.AddressFamily != AddressFamily.InterNetworkV6)
            assertion.Fail(message ?? "IP address must be IPv6.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the endpoint uses the given port.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="port">The expected port.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<IPEndPoint, TPolicy> HavePort<TPolicy>(
        this Assertion<IPEndPoint, TPolicy> assertion,
        int port,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Endpoint must not be null.");

        if (value.Port != port)
            assertion.Fail(message ?? $"Endpoint must have port '{port}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the endpoint address is a loopback address.
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
    public static Assertion<IPEndPoint, TPolicy> Loopback<TPolicy>(
        this Assertion<IPEndPoint, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Endpoint must not be null.");

        if (!IPAddress.IsLoopback(value.Address))
            assertion.Fail(message ?? "Endpoint must be a loopback endpoint.");

        return assertion;
    }
}
