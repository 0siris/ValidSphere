using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides general-purpose fluent assertions.
/// </summary>
public static class CoreAssertions {
    /// <summary>
    ///     Asserts that the subject is equal to the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     Equality is evaluated using <see cref="EqualityComparer{T}.Default" />.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Eq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!EqualityComparer<T>.Default.Equals(assertion.Value, expected)) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"Expected '{expected}', but found '{assertion.Value}'.");
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the subject is equal to the expected value, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Eq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T expected,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!EqualityComparer<T>.Default.Equals(assertion.Value, expected)) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
        }

        return assertion;
    }


    /// <summary>
    ///     Asserts that the subject is not equal to the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the compared values.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="unexpected">The value that must not equal the subject.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotEq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T unexpected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (EqualityComparer<T>.Default.Equals(assertion.Value, unexpected)) {
            TPolicy.Fail(assertion.Context,
                         message ?? $"Value must not be '{unexpected}'.");
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the subject is not equal to the specified value, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> NotEq<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        T unexpected,
        Func<AssertionContext, string> messageFactory
    )
        where TPolicy : struct, IAssertionPolicy {
        if (EqualityComparer<T>.Default.Equals(assertion.Value, unexpected)) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
        }

        return assertion;
    }


    /// <param name="assertion">The current assertion.</param>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    extension<TPolicy>(Assertion<bool, TPolicy> assertion) where TPolicy : struct, IAssertionPolicy {
        /// <summary>
        ///     Asserts that the Boolean subject is <see langword="true" />.
        /// </summary>
        /// <param name="message">
        ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
        /// </param>
        /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
        [DebuggerStepThrough]
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Assertion<bool, TPolicy> True(
            string? message = null
        ) {
            if (!assertion.Value)
                TPolicy.Fail(assertion.Context, message ?? "Expected value to be true.");

            return assertion;
        }
        /// <summary>
        ///     Asserts that the Boolean subject is <see langword="true" />, building the failure message lazily.
        /// </summary>
        /// <remarks>
        ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
        /// </remarks>
        [DebuggerStepThrough]
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Assertion<bool, TPolicy> True(
            Func<AssertionContext, string> messageFactory
        ) {
            if (!assertion.Value)
                TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

            return assertion;
        }


        /// <summary>
        ///     Asserts that the Boolean subject is <see langword="false" />.
        /// </summary>
        /// <param name="message">
        ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
        /// </param>
        /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
        [DebuggerStepThrough]
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Assertion<bool, TPolicy> False(
            string? message = null
        ) {
            if (assertion.Value)
                TPolicy.Fail(assertion.Context, message ?? "Expected value to be false.");

            return assertion;
        }
        /// <summary>
        ///     Asserts that the Boolean subject is <see langword="false" />, building the failure message lazily.
        /// </summary>
        /// <remarks>
        ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
        /// </remarks>
        [DebuggerStepThrough]
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Assertion<bool, TPolicy> False(
            Func<AssertionContext, string> messageFactory
        ) {
            if (assertion.Value)
                TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));

            return assertion;
        }

    }

    /// <summary>
    ///     Asserts that an externally evaluated condition is satisfied.
    /// </summary>
    /// <typeparam name="T">The type of the asserted subject.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="condition">The condition that must evaluate to <see langword="true" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the captured condition expression is used.
    /// </param>
    /// <param name="conditionExpression">
    ///     The source expression of <paramref name="condition" />, supplied automatically by the compiler.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Satisfy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        [DoesNotReturnIf(false)] bool condition,
        string? message = null,
        [CallerArgumentExpression("condition")] string? conditionExpression = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!condition) {
            TPolicy.Fail(assertion.Context,
                         message ?? (conditionExpression is null
                                         ? "Condition was not satisfied."
                                         : $"Condition '{conditionExpression}' was not satisfied."));
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that an externally evaluated condition is satisfied, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    ///     The captured <paramref name="conditionExpression" /> is intentionally unused in this path.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Satisfy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        [DoesNotReturnIf(false)] bool condition,
        Func<AssertionContext, string> messageFactory,
        [CallerArgumentExpression("condition")] string? conditionExpression = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!condition) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
        }

        return assertion;
    }


    /// <summary>
    ///     Asserts that the subject satisfies the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="predicate">
    ///     A predicate that receives the current assertion subject and must return
    ///     <see langword="true" /> for a valid value.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default message containing the captured predicate expression is used.
    /// </param>
    /// <param name="predicateExpression">
    ///     The source expression of <paramref name="predicate" />,
    ///     supplied automatically by the compiler.
    /// </param>
    /// <returns>The assertion for further chaining.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Satisfy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        Func<T, bool> predicate,
        string? message = null,
        [CallerArgumentExpression("predicate")]
        string? predicateExpression = null
    ) where TPolicy : struct, IAssertionPolicy {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!predicate(assertion.Value)) {
            TPolicy.Fail(assertion.Context,
                         message
                         ?? (predicateExpression is null
                                 ? "Subject does not satisfy the required condition."
                                 : $"Subject does not satisfy predicate '{predicateExpression}'."));
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the subject satisfies the specified predicate, building the failure message lazily.
    /// </summary>
    /// <remarks>
    ///     <paramref name="messageFactory" /> is required, receives the call-site context, and is invoked only in the failure branch.
    ///     The captured <paramref name="predicateExpression" /> is intentionally unused in this path.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, TPolicy> Satisfy<T, TPolicy>(
        this Assertion<T, TPolicy> assertion,
        Func<T, bool> predicate,
        Func<AssertionContext, string> messageFactory,
        [CallerArgumentExpression("predicate")]
        string? predicateExpression = null
    ) where TPolicy : struct, IAssertionPolicy {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!predicate(assertion.Value)) {
            TPolicy.Fail(assertion.Context, messageFactory(assertion.Context));
        }

        return assertion;
    }


}
