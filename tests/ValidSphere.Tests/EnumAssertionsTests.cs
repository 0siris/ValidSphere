using ValidSphere;

namespace ValidSphere.Tests;

public sealed class EnumAssertionsTests {
    private enum Color { Red = 1, Blue = 2 }

    [Flags]
    private enum Perm { Read = 1, Write = 2 }

    [Fact]
    public void Flagged_rejects_missing_flag() {
        Assert.Throws<AssertException>(() => { Perm.Read.Is().Flagged(Perm.Write); });
    }

    [Fact]
    public void Flagged_guard_rejects_missing_flag() {
        Assert.ThrowsAny<ArgumentException>(() => { Perm.Read.Guard().Flagged(Perm.Write); });
    }

    [Fact]
    public void Enum_string_rejects_invalid() {
        Assert.Throws<AssertException>(() => { "nope".Is().Enum<Color>(); });
    }

    [Fact]
    public void GuardEnum_string_accepts_valid() {
        Color value = "Red".Guard().GuardEnum<Color>();
        Assert.Equal(Color.Red, value);
    }

    [Fact]
    public void GuardEnum_string_rejects_invalid() {
        Assert.ThrowsAny<ArgumentException>(() => { "nope".Guard().GuardEnum<Color>(); });
    }

    [Fact]
    public void AsEnum_accepts_ignoring_case() {
        Assert.Equal(Color.Red, "red".Is().AsEnum<Color>(ignoreCase: true));
    }

    [Fact]
    public void AsGuardEnum_string_accepts_valid() {
        Assert.Equal(Color.Red, "Red".Guard().AsGuardEnum<Color>());
    }

    [Fact]
    public void Enum_int_accepts_valid() {
        Color value = 2.Is().Enum<Color>();
        Assert.Equal(Color.Blue, value);
    }

    [Fact]
    public void GuardEnum_int_accepts_valid() {
        Color value = 2.Guard().GuardEnum<Color>();
        Assert.Equal(Color.Blue, value);
    }

    [Fact]
    public void GuardEnum_int_rejects_undefined() {
        Assert.ThrowsAny<ArgumentException>(() => { 99.Guard().GuardEnum<Color>(); });
    }

    [Fact]
    public void AsEnum_int_rejects_undefined() {
        Assert.Throws<AssertException>(() => { 99.Is().AsEnum<Color>(); });
    }

    [Fact]
    public void AsGuardEnum_int_accepts_valid() {
        Assert.Equal(Color.Blue, 2.Guard().AsGuardEnum<Color>());
    }

    [Fact]
    public void AsGuardEnum_int_rejects_undefined() {
        Assert.ThrowsAny<ArgumentException>(() => { 99.Guard().AsGuardEnum<Color>(); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { ((Color)99).Is().Defined(_ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        Color.Red.Is().Defined(_ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { ((Color)99).Is().Defined("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
