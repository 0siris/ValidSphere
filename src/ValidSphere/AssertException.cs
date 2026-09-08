using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Marks an exception as an assertion failure for compatible test frameworks.
/// </summary>
/// <remarks>
///     xUnit.net v3 recognizes exceptions implementing an interface named
///     <c>IAssertionException</c> as assertion failures without requiring this library to reference xUnit.net.
/// </remarks>
public interface IAssertionException;

/// <summary>
///     Represents a failed runtime assertion.
/// </summary>
/// <summary>
///     Represents a failed runtime assertion.
/// </summary>
public sealed class AssertException : InvalidOperationException, IAssertionException {
    /// <summary>
    ///     Initializes a new instance of the <see cref="AssertException" /> class.
    /// </summary>
    /// <param name="message">The assertion failure message.</param>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    public AssertException(string message, AssertionContext context)
        : base(AssertionFailureMessage.Format(message, context))
        => Context = context;

    /// <summary>
    ///     Gets the compiler-provided call-site context for the failed assertion.
    /// </summary>
    public AssertionContext Context { get; }

    /// <summary>
    ///     Gets the expression that produced the value for which the assertion failed.
    /// </summary>
    public string? Expression => Context.Expression;
}

/// <summary>
///     Formats failure messages with compiler-provided assertion call-site information.
/// </summary>
internal static class AssertionFailureMessage {
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static string Format(string message, AssertionContext context) {
        var expression = context.Expression;
        var memberName = context.MemberName;
        var filePath = context.FilePath;
        var lineNumber = context.LineNumber;

        if (expression is null && memberName is null && filePath is null && lineNumber <= 0)
            return message;

        var result = message;

        if (expression is not null)
            result += $"{Environment.NewLine}Expression: {expression}";

        if (memberName is not null)
            result += $"{Environment.NewLine}Member: {memberName}";

        if (filePath is not null) {
            result += lineNumber > 0
                          ? $"{Environment.NewLine}Source: {filePath}({lineNumber})"
                          : $"{Environment.NewLine}Source: {filePath}";
        }
        else if (lineNumber > 0) {
            result += $"{Environment.NewLine}Line: {lineNumber}";
        }

        return result;
    }
}
