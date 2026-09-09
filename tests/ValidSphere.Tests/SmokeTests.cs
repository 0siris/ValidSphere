using ValidSphere;

namespace ValidSphere.Tests;

public sealed class SmokeTests {
    [Fact]
    public void AsNotNull_returns_value() {
        string? name = "hello";
        Assert.Equal("hello", name.AsNotNull());
    }

    [Fact]
    public void AsNotNull_null_throws_assert() {
        string? name = null;
        Assert.Throws<AssertException>(() => { name.AsNotNull(); });
    }

    [Fact]
    public void AsGuardNotNull_null_throws_argumentnull() {
        string? name = null;
        Assert.Throws<ArgumentNullException>(() => { name.AsGuardNotNull(); });
    }

    [Fact]
    public void Guid_entry_chains() {
        "3f2504e0-4f89-11d3-9a0c-0305e82c3301".Is().Guid().NotEmpty();
    }

    [Fact]
    public void AsGuid_extracts_raw() {
        var sample = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
        Guid parsed = sample.Is().AsGuid();
        Assert.Equal(Guid.Parse(sample), parsed);
    }
}
