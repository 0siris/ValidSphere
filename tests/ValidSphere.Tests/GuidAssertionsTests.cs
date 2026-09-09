using ValidSphere;

namespace ValidSphere.Tests;

public sealed class GuidAssertionsTests {
    private const string Sample = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";

    [Fact]
    public void NotEmpty_accepts_value() {
        Guid? value = Guid.NewGuid();
        value.Is().NotNull().NotEmpty();
    }

    [Fact]
    public void NotEmpty_rejects_empty() {
        Guid? value = Guid.Empty;
        Assert.Throws<AssertException>(() => { value.Is().NotNull().NotEmpty(); });
    }

    [Fact]
    public void NotEmpty_guard_rejects_empty() {
        Guid? value = Guid.Empty;
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNull().NotEmpty(); });
    }

    [Fact]
    public void NotNullOrEmpty_accepts_value() {
        Guid? value = Guid.NewGuid();
        Assert.Equal(value.Value, value.Is().NotNullOrEmpty().Value);
    }

    [Fact]
    public void NotNullOrEmpty_rejects_null() {
        Guid? value = null;
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrEmpty(); });
    }

    [Fact]
    public void NotNullOrEmpty_rejects_empty() {
        Guid? value = Guid.Empty;
        Assert.Throws<AssertException>(() => { value.Is().NotNullOrEmpty(); });
    }

    [Fact]
    public void NotNullOrEmpty_guard_rejects_null() {
        Guid? value = null;
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNullOrEmpty(); });
    }

    [Fact]
    public void Guid_parses_and_refines() {
        Guid parsed = Sample.Is().Guid();
        Assert.Equal(Guid.Parse(Sample), parsed);
        Sample.Is().Guid().NotEmpty();
    }

    [Fact]
    public void Guid_rejects_invalid() {
        Assert.Throws<AssertException>(() => { "nope".Is().Guid(); });
    }

    [Fact]
    public void Guid_guard_rejects_invalid() {
        Assert.ThrowsAny<ArgumentException>(() => { "nope".Guard().Guid(); });
    }

    [Fact]
    public void AsGuid_extracts_value() {
        Assert.Equal(Guid.Parse(Sample), Sample.Is().AsGuid());
    }

    [Fact]
    public void AsGuid_rejects_invalid() {
        Assert.Throws<AssertException>(() => { "nope".Is().AsGuid(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        Guid? value = Guid.Empty;
        var ex = Assert.Throws<AssertException>(() => { value.Is().NotNull().NotEmpty("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
