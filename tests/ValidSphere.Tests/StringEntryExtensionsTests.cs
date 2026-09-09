using ValidSphere;

namespace ValidSphere.Tests;

public sealed class StringEntryExtensionsTests {
    [Fact]
    public void AsNotNullOrEmpty_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsNotNullOrEmpty());
    }

    [Fact]
    public void AsNotNullOrEmpty_rejects_empty() {
        string? value = "";
        Assert.Throws<AssertException>(() => { value.AsNotNullOrEmpty(); });
    }

    [Fact]
    public void AsNotNullOrEmpty_rejects_null() {
        string? value = null;
        Assert.Throws<AssertException>(() => { value.AsNotNullOrEmpty(); });
    }

    [Fact]
    public void AsNotNullOrWhiteSpace_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsNotNullOrWhiteSpace());
    }

    [Fact]
    public void AsNotNullOrWhiteSpace_rejects_whitespace() {
        string? value = "  ";
        Assert.Throws<AssertException>(() => { value.AsNotNullOrWhiteSpace(); });
    }

    [Fact]
    public void AsGuardNotNullOrEmpty_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsGuardNotNullOrEmpty());
    }

    [Fact]
    public void AsGuardNotNullOrEmpty_rejects_empty() {
        string? value = "";
        Assert.ThrowsAny<ArgumentException>(() => { value.AsGuardNotNullOrEmpty(); });
    }

    [Fact]
    public void AsGuardNotNullOrWhiteSpace_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsGuardNotNullOrWhiteSpace());
    }

    [Fact]
    public void AsGuardNotNullOrWhiteSpace_rejects_whitespace() {
        string? value = "  ";
        Assert.ThrowsAny<ArgumentException>(() => { value.AsGuardNotNullOrWhiteSpace(); });
    }

    [Fact]
    public void AsNotEmpty_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsNotEmpty());
    }

    [Fact]
    public void AsNotEmpty_rejects_empty() {
        string? value = "";
        Assert.Throws<AssertException>(() => { value.AsNotEmpty(); });
    }

    [Fact]
    public void AsGuardNotEmpty_returns_value() {
        string? value = "x";
        Assert.Equal("x", value.AsGuardNotEmpty());
    }

    [Fact]
    public void AsGuardNotEmpty_rejects_empty() {
        string? value = "";
        Assert.ThrowsAny<ArgumentException>(() => { value.AsGuardNotEmpty(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        string? value = "";
        var ex = Assert.Throws<AssertException>(() => { value.AsNotNullOrEmpty("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
