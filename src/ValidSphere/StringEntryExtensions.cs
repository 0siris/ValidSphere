using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides flow-analysis-aware entry points for non-empty string validation.
/// </summary>
/// <remarks>
///     Combines the null check of <c>AsGuardNotNull()</c>/<c>AsNotNull()</c> with the content checks of
///     <c>NotNullOrEmpty()</c>/<c>NotNullOrWhiteSpace()</c>, so the original variable is known to be non-null
///     after a successful call.
/// </remarks>
public static class StringEntryExtensions {
    /// <summary>
    ///     Asserts that the specified string is neither <see langword="null" /> nor empty.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default assertion message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="AssertException">
    ///     <paramref name="subject" /> is <see langword="null" /> or empty.
    /// </exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsNotNullOrEmpty(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            IsPolicy.FailNull(context, message ?? "String must not be null.");

        if (subject.Length == 0)
            IsPolicy.Fail(context, message ?? "String must not be empty.");

        return subject;
    }

    /// <summary>
    ///     Asserts that the specified string is neither <see langword="null" />, empty,
    ///     nor composed exclusively of white-space characters.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default assertion message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="AssertException">
    ///     <paramref name="subject" /> is <see langword="null" />, empty, or whitespace.
    /// </exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsNotNullOrWhiteSpace(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            IsPolicy.FailNull(context, message ?? "String must not be null.");

        if (string.IsNullOrWhiteSpace(subject))
            IsPolicy.Fail(context, message ?? "String must not be empty or whitespace.");

        return subject;
    }

    /// <summary>
    ///     Validates that the specified string argument is neither <see langword="null" /> nor empty.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default guard message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="subject" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="subject" /> is empty.
    /// </exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsGuardNotNullOrEmpty(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            GuardPolicy.FailNull(context, message ?? "String must not be null.");

        if (subject.Length == 0)
            GuardPolicy.Fail(context, message ?? "String must not be empty.");

        return subject;
    }

    /// <summary>
    ///     Validates that the specified string argument is neither <see langword="null" />, empty,
    ///     nor composed exclusively of white-space characters.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default guard message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="subject" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="subject" /> is empty or whitespace.
    /// </exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsGuardNotNullOrWhiteSpace(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            GuardPolicy.FailNull(context, message ?? "String must not be null.");

        if (string.IsNullOrWhiteSpace(subject))
            GuardPolicy.Fail(context, message ?? "String must not be empty or whitespace.");

        return subject;
    }

    /// <summary>
    ///     Validates that the specified string is not empty and returns it.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default assertion message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="AssertException">
    ///     <paramref name="subject" /> is <see langword="null" /> or empty.
    /// </exception>
    /// <remarks>
    ///     Terminal extractor: for a chainable check use <c>Is().NotEmpty()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsNotEmpty(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            IsPolicy.FailNull(context, message ?? "String must not be null.");

        if (subject.Length == 0)
            IsPolicy.Fail(context, message ?? "String must not be empty.");

        return subject;
    }

    /// <summary>
    ///     Validates that the specified string argument is not empty and returns it.
    /// </summary>
    /// <param name="subject">
    ///     The string to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default guard message is used.
    /// </param>
    /// <returns>
    ///     The validated non-null string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="subject" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="subject" /> is empty.
    /// </exception>
    /// <remarks>
    ///     Terminal extractor: for a chainable check use <c>Guard().NotEmpty()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsGuardNotEmpty(
        [NotNull] this string? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            GuardPolicy.FailNull(context, message ?? "String must not be null.");

        if (subject.Length == 0)
            GuardPolicy.Fail(context, message ?? "String must not be empty.");

        return subject;
    }
}
