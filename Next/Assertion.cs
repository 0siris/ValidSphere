using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Assertions.Next;

/// <summary>
///     Represents a lightweight fluent assertion for a value.
/// </summary>
/// <typeparam name="T">The type of the asserted value.</typeparam>
/// <typeparam name="TPolicy">
///     The failure policy that determines which exception type is thrown when an assertion fails.
/// </typeparam>
/// <remarks>
///     This type is implemented as a <see langword="readonly struct" /> to avoid heap allocations
///     on the successful assertion path. Its public constructor and properties also form the minimal
///     extension surface for custom assertion libraries.
/// </remarks>
public readonly struct Assertion<T, TPolicy>
    where TPolicy : struct, IAssertionPolicy {

    private readonly T subject;
    private readonly AssertionContext context;

    /// <summary>
    ///     Initializes a new assertion for the specified subject and call-site context.
    /// </summary>
    /// <param name="subject">The value to assert.</param>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    /// <remarks>
    ///     This constructor is public so custom assertion extensions can refine an assertion to another type
    ///     while preserving the original call-site information.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Assertion(T subject, AssertionContext context) {
        this.subject = subject;
        this.context = context;
    }

    /// <summary>
    ///     Gets the asserted value.
    /// </summary>
    public T Value {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => subject;
    }

    /// <summary>
    ///     Gets the compiler-provided call-site context captured by <c>Should()</c> or <c>Guard()</c>.
    /// </summary>
    public AssertionContext Context {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => context;
    }

    /// <summary>
    ///     Gets the source expression that produced the asserted value.
    /// </summary>
    public string? Expression {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => context.Expression;
    }

    /// <summary>
    ///     Asserts that the subject is compatible with <typeparamref name="TExpected" /> and refines
    ///     the assertion to that type.
    /// </summary>
    /// <typeparam name="TExpected">The required runtime type.</typeparam>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>An assertion whose subject is typed as <typeparamref name="TExpected" />.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Assertion<TExpected, TPolicy> BeOfType<TExpected>(string? message = null) {
        if (subject is TExpected typed)
            return new Assertion<TExpected, TPolicy>(typed, context);

        TPolicy.Fail(context,
                     message ?? $"Value must be of type '{typeof(TExpected).Name}'.");

        return default;
    }

    /// <summary>
    ///     Implicitly extracts the asserted value.
    /// </summary>
    /// <param name="assertion">The assertion whose value is returned.</param>
    /// <returns>The asserted value.</returns>
    /// <remarks>
    ///     This allows assertions to be embedded directly into expressions, for example:
    ///     <c>Use(value.Guard().BeGreaterThan(0))</c>.
    /// </remarks>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Assertion<T, TPolicy> assertion)
        => assertion.subject;
}


/// <summary>
///     Provides entry points for fluent assertions and argument guards.
/// </summary>
public static class AssertionEntryExtensions {
    /// <summary>
    ///     Starts a fluent runtime assertion for the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the asserted value.</typeparam>
    /// <param name="subject">The value to assert.</param>
    /// <param name="expression">
    ///     The source expression for <paramref name="subject" />, supplied automatically by the compiler.
    /// </param>
    /// <param name="memberName">The caller member name, supplied automatically by the compiler.</param>
    /// <param name="filePath">The caller source file path, supplied automatically by the compiler.</param>
    /// <param name="lineNumber">The caller source line number, supplied automatically by the compiler.</param>
    /// <returns>
    ///     An assertion using <see cref="ShouldPolicy" />, which throws <see cref="AssertException" />
    ///     when a constraint fails.
    /// </returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, ShouldPolicy> Should<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    )
        => new(subject, new AssertionContext(expression, memberName, filePath, lineNumber));

    /// <summary>
    ///     Starts a fluent argument or precondition guard for the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the guarded argument.</typeparam>
    /// <param name="subject">The argument value to validate.</param>
    /// <param name="expression">
    ///     The source expression for <paramref name="subject" />, supplied automatically by the compiler
    ///     and used as the argument name when possible.
    /// </param>
    /// <param name="memberName">The caller member name, supplied automatically by the compiler.</param>
    /// <param name="filePath">The caller source file path, supplied automatically by the compiler.</param>
    /// <param name="lineNumber">The caller source line number, supplied automatically by the compiler.</param>
    /// <returns>
    ///     An assertion using <see cref="GuardPolicy" />, which throws standard argument exceptions
    ///     when a constraint fails.
    /// </returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, GuardPolicy> Guard<T>(
        this T subject,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    )
        => new(subject, new AssertionContext(expression, memberName, filePath, lineNumber));


    /// <summary>
    ///     Asserts that the specified reference is not <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The reference type of the asserted value.</typeparam>
    /// <param name="subject">
    ///     The reference to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default assertion message is used.
    /// </param>
    /// <param name="expression">
    ///     The source expression that produced <paramref name="subject" />,
    ///     supplied automatically by the compiler.
    /// </param>
    /// <param name="memberName">
    ///     The name of the calling member, supplied automatically by the compiler.
    /// </param>
    /// <param name="filePath">
    ///     The source file path of the caller, supplied automatically by the compiler.
    /// </param>
    /// <param name="lineNumber">
    ///     The source line number of the caller, supplied automatically by the compiler.
    /// </param>
    /// <returns>
    ///     An assertion containing the validated non-null value for further fluent assertions
    ///     or implicit extraction of the value.
    /// </returns>
    /// <exception cref="AssertException">
    ///     <paramref name="subject"/> is <see langword="null" />.
    /// </exception>
    /// <remarks>
    ///     Unlike <c>Should().NotBeNull()</c>, this direct assertion participates in nullable
    ///     flow analysis through <see cref="NotNullAttribute"/> and therefore refines the
    ///     original reference to non-null after a successful call.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, ShouldPolicy> AssertNotNull<T>(
        [NotNull] this T? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    )
        where T : class
    {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            ShouldPolicy.FailNull(context, message ?? "Value must not be null");

        return new(subject, context);
    }

    /// <summary>
    ///     Asserts that the specified nullable value type contains a value.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="subject">
    ///     The nullable value to validate. When this method returns normally,
    ///     the compiler considers the value to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default assertion message is used.
    /// </param>
    /// <param name="expression">
    ///     The source expression that produced <paramref name="subject" />,
    ///     supplied automatically by the compiler.
    /// </param>
    /// <param name="memberName">
    ///     The name of the calling member, supplied automatically by the compiler.
    /// </param>
    /// <param name="filePath">
    ///     The source file path of the caller, supplied automatically by the compiler.
    /// </param>
    /// <param name="lineNumber">
    ///     The source line number of the caller, supplied automatically by the compiler.
    /// </param>
    /// <returns>
    ///     An assertion containing the underlying non-null value.
    /// </returns>
    /// <exception cref="AssertException">
    ///     <paramref name="subject"/> has no value.
    /// </exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, ShouldPolicy> AssertNotNull<T>(
        [NotNull] this T? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    )
        where T : struct {
        var context = new AssertionContext(
                                           expression,
                                           memberName,
                                           filePath,
                                           lineNumber);

        if (!subject.HasValue)
            ShouldPolicy.FailNull(
                                  context,
                                  message ?? "Value must have a value.");

        return new(subject.GetValueOrDefault(), context);
    }


    /// <summary>
    ///     Validates that the specified argument or precondition value is not
    ///     <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The reference type of the guarded value.</typeparam>
    /// <param name="subject">
    ///     The reference to validate. When this method returns normally, the compiler
    ///     considers the reference to be non-null.
    /// </param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />,
    ///     a default guard message is used.
    /// </param>
    /// <param name="expression">
    ///     The source expression that produced <paramref name="subject" />,
    ///     supplied automatically by the compiler and used as the argument name.
    /// </param>
    /// <param name="memberName">
    ///     The name of the calling member, supplied automatically by the compiler.
    /// </param>
    /// <param name="filePath">
    ///     The source file path of the caller, supplied automatically by the compiler.
    /// </param>
    /// <param name="lineNumber">
    ///     The source line number of the caller, supplied automatically by the compiler.
    /// </param>
    /// <returns>
    ///     An assertion containing the validated non-null value for further fluent assertions
    ///     or implicit extraction of the value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="subject"/> is <see langword="null" />.
    /// </exception>
    /// <remarks>
    ///     This is the flow-analysis-aware counterpart to <c>Guard().NotBeNull()</c>.
    ///     The <see cref="NotNullAttribute"/> informs nullable flow analysis that
    ///     <paramref name="subject"/> is non-null after a successful call.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<T, GuardPolicy> GuardNotNull<T>(
        [NotNull] this T? subject,
        string? message = null,
        [CallerArgumentExpression("subject")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0
    )
        where T : class {
        var context = new AssertionContext(expression, memberName, filePath, lineNumber);
        if (subject is null)
            GuardPolicy.FailNull(context, message ?? "Value must not be null");

        return new(subject, context);
    }

    /// <summary>
///     Validates that the specified nullable value-type argument contains a value.
/// </summary>
/// <typeparam name="T">The underlying value type.</typeparam>
/// <param name="subject">
///     The nullable value to validate. When this method returns normally,
///     the compiler considers the value to be non-null.
/// </param>
/// <param name="message">
///     An optional custom failure message. When <see langword="null" />,
///     a default guard message is used.
/// </param>
/// <param name="expression">
///     The source expression that produced <paramref name="subject" />,
///     supplied automatically by the compiler and used as the argument name.
/// </param>
/// <param name="memberName">
///     The name of the calling member, supplied automatically by the compiler.
/// </param>
/// <param name="filePath">
///     The source file path of the caller, supplied automatically by the compiler.
/// </param>
/// <param name="lineNumber">
///     The source line number of the caller, supplied automatically by the compiler.
/// </param>
/// <returns>
///     An assertion containing the underlying non-null value.
/// </returns>
/// <exception cref="ArgumentNullException">
///     <paramref name="subject"/> has no value.
/// </exception>
[DebuggerStepThrough]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static Assertion<T, GuardPolicy> GuardNotNull<T>(
    [NotNull] this T? subject,
    string? message = null,
    [CallerArgumentExpression("subject")] string? expression = null,
    [CallerMemberName] string? memberName = null,
    [CallerFilePath] string? filePath = null,
    [CallerLineNumber] int lineNumber = 0
)
    where T : struct {
    var context = new AssertionContext(
        expression,
        memberName,
        filePath,
        lineNumber);

    if (!subject.HasValue)
        GuardPolicy.FailNull(
            context,
            message ?? "Value must have a value.");

    return new(subject.GetValueOrDefault(), context);
}
}