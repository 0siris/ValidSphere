namespace ValidSphere;

/// <summary>
///     Describes the semantic category of an assertion failure.
/// </summary>
public enum AssertionFailureKind : byte {
    /// <summary>A general assertion failure.</summary>
    General,
    /// <summary>The asserted value was unexpectedly <see langword="null" />.</summary>
    Null,
    /// <summary>The asserted value was outside the allowed range.</summary>
    OutOfRange
}

/// <summary>
///     Describes a single assertion failure for custom exception factories.
/// </summary>
public readonly struct AssertionFailure {
    /// <summary>
    ///     Initializes a new assertion failure description.
    /// </summary>
    /// <param name="kind">The semantic failure category.</param>
    /// <param name="message">The failure message.</param>
    /// <param name="context">The compiler-provided assertion call-site context.</param>
    /// <param name="actualValue">The offending value, where applicable.</param>
    public AssertionFailure(
        AssertionFailureKind kind,
        string message,
        AssertionContext context,
        object? actualValue = null) {
        Kind = kind;
        Message = message;
        Context = context;
        ActualValue = actualValue;
    }

    /// <summary>Gets the semantic failure category.</summary>
    public AssertionFailureKind Kind { get; }

    /// <summary>Gets the failure message.</summary>
    public string Message { get; }

    /// <summary>Gets the compiler-provided assertion call-site context.</summary>
    public AssertionContext Context { get; }

    /// <summary>Gets the offending value, where applicable.</summary>
    public object? ActualValue { get; }
}
