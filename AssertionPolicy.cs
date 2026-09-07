using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Assertions;

/// <summary>
///     Defines the failure behavior used by fluent assertions.
/// </summary>
/// <remarks>
///     Implementations are stateless value types. Static abstract members allow assertion extensions
///     to select the appropriate failure behavior without reflection, allocation, or virtual dispatch.
/// </remarks>
public interface IAssertionPolicy {
    /// <summary>
    ///     Throws an exception for a general assertion failure.
    /// </summary>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void Fail(AssertionContext context, string message);

    /// <summary>
    ///     Throws an exception when an asserted value is unexpectedly <see langword="null" />.
    /// </summary>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void FailNull(AssertionContext context, string message);

    /// <summary>
    ///     Throws an exception when an asserted value is outside an allowed range.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    /// <param name="actualValue">The value that violated the range constraint.</param>
    /// <param name="message">The failure message.</param>
    [DoesNotReturn]
    static abstract void FailOutOfRange<T>(AssertionContext context, T actualValue, string message);
}


/// <summary>
///     Failure policy used by <c>Should()</c>.
/// </summary>
/// <remarks>
///     Failed assertions are reported as <see cref="AssertException" /> instances.
/// </remarks>
public readonly struct ShouldPolicy : IAssertionPolicy {
    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(AssertionContext context, string message) {
        throw new AssertException(message, context);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(AssertionContext context, string message) {
        throw new AssertException(message, context);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(AssertionContext context, T actualValue, string message) {
        throw new AssertException(message, context);
    }
}

/// <summary>
///     Failure policy used by <c>Guard()</c>.
/// </summary>
/// <remarks>
///     Guard failures use the standard .NET argument exception hierarchy:
///     <see cref="ArgumentException" />, <see cref="ArgumentNullException" />,
///     or <see cref="ArgumentOutOfRangeException" />.
/// </remarks>
public readonly struct GuardPolicy : IAssertionPolicy {
    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Fail(AssertionContext context, string message) {
        throw new ArgumentException(AssertionFailureMessage.Format(message, context), context.Expression);
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailNull(AssertionContext context, string message) {
        throw new ArgumentNullException(context.Expression, AssertionFailureMessage.Format(message, context));
    }

    /// <inheritdoc />
    [DoesNotReturn]
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void FailOutOfRange<T>(AssertionContext context, T actualValue, string message) {
        throw new ArgumentOutOfRangeException(context.Expression,
                                              actualValue,
                                              AssertionFailureMessage.Format(message, context));
    }
}