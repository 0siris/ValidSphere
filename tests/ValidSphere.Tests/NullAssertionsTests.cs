using ValidSphere;

namespace ValidSphere.Tests;

public sealed class NullAssertionsTests {
    [Fact]
    public void Class_NotNull_accepts_value() {
        string? value = "x";
        Assert.Equal("x", value.Is().NotNull().Value);
    }

    [Fact]
    public void Class_NotNull_rejects_null() {
        string? value = null;
        Assert.Throws<AssertException>(() => { value.Is().NotNull(); });
    }

    [Fact]
    public void Class_NotNull_guard_rejects_null() {
        string? value = null;
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNull(); });
    }

    [Fact]
    public void Struct_NotNull_accepts_value() {
        int? value = 5;
        Assert.Equal(5, value.Is().NotNull().Value);
    }

    [Fact]
    public void Struct_NotNull_rejects_null() {
        int? value = null;
        Assert.Throws<AssertException>(() => { value.Is().NotNull(); });
    }

    [Fact]
    public void Struct_NotNull_guard_rejects_null() {
        int? value = null;
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().NotNull(); });
    }

    [Fact]
    public void Class_Null_accepts_null() {
        string? value = null;
        value.Is().Null();
    }

    [Fact]
    public void Class_Null_rejects_value() {
        string? value = "x";
        Assert.Throws<AssertException>(() => { value.Is().Null(); });
    }

    [Fact]
    public void Class_Null_guard_rejects_value() {
        string? value = "x";
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().Null(); });
    }

    [Fact]
    public void Struct_Null_accepts_null() {
        int? value = null;
        value.Is().Null();
    }

    [Fact]
    public void Struct_Null_rejects_value() {
        int? value = 5;
        Assert.Throws<AssertException>(() => { value.Is().Null(); });
    }

    [Fact]
    public void Struct_Null_guard_rejects_value() {
        int? value = 5;
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().Null(); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        string? value = null;
        var ex = Assert.Throws<AssertException>(() => { value.Is().NotNull(_ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        string? ok = "x";
        ok.Is().NotNull(_ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        string? value = null;
        var ex = Assert.Throws<AssertException>(() => { value.Is().NotNull("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
