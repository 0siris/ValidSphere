using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Assertions.Next;

/// <summary>
///     Contains compiler-provided source information for an assertion call site.
/// </summary>
/// <remarks>
///     The context stores only value types and references to compiler-generated string literals.
///     Capturing it does not require reflection, stack-trace inspection, or heap allocation.
/// </remarks>
public readonly struct AssertionContext {
    /// <summary>
    ///     Initializes a new assertion context.
    /// </summary>
    /// <param name="expression">The source expression that produced the asserted value.</param>
    /// <param name="memberName">The caller member containing the assertion.</param>
    /// <param name="filePath">The source file containing the assertion.</param>
    /// <param name="lineNumber">The source line containing the assertion.</param>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AssertionContext(
        string? expression,
        string? memberName,
        string? filePath,
        int lineNumber
    ) {
        Expression = expression;
        MemberName = memberName;
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    /// <summary>Gets the source expression that produced the asserted value.</summary>
    public string? Expression { get; }

    /// <summary>Gets the caller member containing the assertion.</summary>
    public string? MemberName { get; }

    /// <summary>Gets the source file containing the assertion.</summary>
    public string? FilePath { get; }

    /// <summary>Gets the source line containing the assertion.</summary>
    public int LineNumber { get; }
}