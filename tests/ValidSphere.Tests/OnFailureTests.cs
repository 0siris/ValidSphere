using ValidSphere;

namespace ValidSphere.Tests;

public sealed class OnFailureTests {
    private sealed class DomainException : Exception {
        public DomainException() { }

        public DomainException(string message)
            : base(message) { }
    }

    [Fact]
    public void Default_Is_Greater_throws_AssertException() {
        Assert.Throws<AssertException>(() => { 0.Is().Greater(0); });
    }

    [Fact]
    public void Default_Guard_Greater_throws_ArgumentOutOfRange() {
        Assert.Throws<ArgumentOutOfRangeException>(() => { 0.Guard().Greater(0); });
    }

    [Fact]
    public void Custom_Is_chain_throws_DomainException() {
        Assert.Throws<DomainException>(() => { 0.Is().OnFailure(static _ => new DomainException()).Greater(0); });
    }

    [Fact]
    public void Custom_Guard_chain_throws_DomainException() {
        Assert.Throws<DomainException>(() => { 0.Guard().OnFailure(static _ => new DomainException()).Greater(0); });
    }

    [Fact]
    public void Metadata_OutOfRange_reports_kind_message_actual_and_context() {
        AssertionFailure? captured = null;
        Assert.Throws<DomainException>(() => {
            5.Is().OnFailure(f => { captured = f; return new DomainException(f.Message); }).Greater(10);
        });

        Assert.True(captured.HasValue);
        var failure = captured!.Value;
        Assert.Equal(AssertionFailureKind.OutOfRange, failure.Kind);
        Assert.Equal("Value must be greater than '10'.", failure.Message);
        Assert.Equal(5, failure.ActualValue);
        Assert.Equal("5", failure.Context.Expression);
        Assert.Equal(nameof(Metadata_OutOfRange_reports_kind_message_actual_and_context), failure.Context.MemberName);
        Assert.EndsWith("OnFailureTests.cs", failure.Context.FilePath);
        Assert.True(failure.Context.LineNumber > 0);
    }

    [Fact]
    public void Metadata_Null_reports_null_kind() {
        AssertionFailure? captured = null;
        string? value = null;
        Assert.Throws<DomainException>(() => {
            value.Is().OnFailure(f => { captured = f; return new DomainException(f.Message); }).NotNull();
        });

        Assert.True(captured.HasValue);
        var failure = captured!.Value;
        Assert.Equal(AssertionFailureKind.Null, failure.Kind);
        Assert.Equal("Value must not be null.", failure.Message);
        Assert.Null(failure.ActualValue);
    }

    [Fact]
    public void Metadata_General_reports_general_kind() {
        AssertionFailure? captured = null;
        Assert.Throws<DomainException>(() => {
            "y".Is().OnFailure(f => { captured = f; return new DomainException(f.Message); }).Eq("x");
        });

        Assert.True(captured.HasValue);
        var failure = captured!.Value;
        Assert.Equal(AssertionFailureKind.General, failure.Kind);
        Assert.Equal("Expected 'x', but found 'y'.", failure.Message);
    }

    [Fact]
    public void Factory_survives_NotNull_refinement() {
        string? value = "";
        Assert.Throws<DomainException>(() => { value.Is().OnFailure(static _ => new DomainException()).NotNull().NotEmpty(); });
    }

    [Fact]
    public void Factory_survives_string_refinement_chain() {
        string? value = "x";
        Assert.Throws<DomainException>(() => { value.Is().OnFailure(static _ => new DomainException()).NotNullOrWhiteSpace().Length(5); });
    }

    [Fact]
    public void Factory_survives_OfType_refinement() {
        object value = "";
        Assert.Throws<DomainException>(() => { value.Is().OnFailure(static _ => new DomainException()).OfType<string>().NotEmpty(); });
    }

    [Fact]
    public void Factory_not_invoked_on_success() {
        var invoked = false;
        5.Is().OnFailure(_ => { invoked = true; return new DomainException(); }).Greater(0).Less(100);
        Assert.False(invoked);
    }
}
