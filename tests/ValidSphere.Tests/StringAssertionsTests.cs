using System.Text.RegularExpressions;
using ValidSphere;

namespace ValidSphere.Tests;

public sealed class StringAssertionsTests {
    [Fact]
    public void NotNullOrEmpty_accepts_value() {
        string? value = "x";
        Assert.Equal("x", value.Is().NotNullOrEmpty().Value);
    }

    [Fact]
    public void NotNullOrEmpty_rejects_empty() {
        string? value = "";
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrEmpty(); });
    }

    [Fact]
    public void NotNullOrEmpty_rejects_null() {
        string? value = null;
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrEmpty(); });
    }

    [Fact]
    public void NotNullOrEmpty_guard_rejects_empty() {
        string? value = "";
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNullOrEmpty(); });
    }

    [Fact]
    public void NotNullOrWhiteSpace_accepts_value() {
        string? value = "x";
        Assert.Equal("x", value.Is().NotNullOrWhiteSpace().Value);
    }

    [Fact]
    public void NotNullOrWhiteSpace_rejects_whitespace() {
        string? value = "  ";
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrWhiteSpace(); });
    }

    [Fact]
    public void NotNullOrWhiteSpace_rejects_null() {
        string? value = null;
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrWhiteSpace(); });
    }

    [Fact]
    public void NotNullOrWhiteSpace_guard_rejects_whitespace() {
        string? value = "  ";
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNullOrWhiteSpace(); });
    }

    [Fact]
    public void NotEmpty_accepts_value() {
        "x".Is().NotNull().NotEmpty();
    }

    [Fact]
    public void NotEmpty_rejects_empty() {
        Assert.Throws<AssertException>(() => { "".Is().NotNull().NotEmpty(); });
    }

    [Fact]
    public void NotEmpty_guard_rejects_empty() {
        Assert.ThrowsAny<ArgumentException>(() => { "".Guard().NotNull().NotEmpty(); });
    }

    [Fact]
    public void Length_accepts_exact() {
        "abc".Is().NotNull().Length(3);
    }

    [Fact]
    public void Length_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().Length(5); });
    }

    [Fact]
    public void Length_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().Length(5); });
    }

    [Fact]
    public void MinLength_accepts_longer() {
        "abc".Is().NotNull().MinLength(2);
    }

    [Fact]
    public void MinLength_rejects_shorter() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().MinLength(5); });
    }

    [Fact]
    public void MinLength_guard_rejects_shorter() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().MinLength(5); });
    }

    [Fact]
    public void MaxLength_accepts_shorter() {
        "abc".Is().NotNull().MaxLength(5);
    }

    [Fact]
    public void MaxLength_rejects_longer() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().MaxLength(2); });
    }

    [Fact]
    public void MaxLength_guard_rejects_longer() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().MaxLength(2); });
    }

    [Fact]
    public void LengthInRange_accepts_inside() {
        "abc".Is().NotNull().LengthInRange(1, 5);
    }

    [Fact]
    public void LengthInRange_rejects_outside() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().LengthInRange(5, 9); });
    }

    [Fact]
    public void LengthInRange_guard_rejects_outside() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().LengthInRange(5, 9); });
    }

    [Fact]
    public void Contains_accepts_substring() {
        "abc".Is().NotNull().Contains("b");
    }

    [Fact]
    public void Contains_accepts_ignoring_case() {
        "abc".Is().NotNull().Contains("B", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Contains_rejects_missing() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().Contains("z"); });
    }

    [Fact]
    public void Contains_guard_rejects_missing() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().Contains("z"); });
    }

    [Fact]
    public void StartsWith_accepts_prefix() {
        "abc".Is().NotNull().StartsWith("a");
    }

    [Fact]
    public void StartsWith_accepts_ignoring_case() {
        "abc".Is().NotNull().StartsWith("A", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StartsWith_rejects_other() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().StartsWith("b"); });
    }

    [Fact]
    public void StartsWith_guard_rejects_other() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().StartsWith("b"); });
    }

    [Fact]
    public void EndsWith_accepts_suffix() {
        "abc".Is().NotNull().EndsWith("c");
    }

    [Fact]
    public void EndsWith_accepts_ignoring_case() {
        "abc".Is().NotNull().EndsWith("C", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EndsWith_rejects_other() {
        Assert.Throws<AssertException>(() => { "abc".Is().NotNull().EndsWith("b"); });
    }

    [Fact]
    public void EndsWith_guard_rejects_other() {
        Assert.ThrowsAny<ArgumentException>(() => { "abc".Guard().NotNull().EndsWith("b"); });
    }

    [Fact]
    public void Matches_accepts_match() {
        "aaa".Is().NotNull().Matches(new Regex("^a+$"));
    }

    [Fact]
    public void Matches_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { "b".Is().NotNull().Matches(new Regex("^a+$")); });
    }

    [Fact]
    public void Matches_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { "b".Guard().NotNull().Matches(new Regex("^a+$")); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { "".Is().NotNull().NotEmpty(_ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        "x".Is().NotNull().NotEmpty(_ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { "".Is().NotNull().NotEmpty("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
