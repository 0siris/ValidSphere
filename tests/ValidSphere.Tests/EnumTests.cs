using ValidSphere;

namespace ValidSphere.Tests;

public sealed class EnumTests {
    private enum Color { Red = 1, Blue = 2 }

    [Flags]
    private enum Perm { Read = 1, Write = 2 }

    [Fact]
    public void Defined_accepts_defined_value() {
        Color.Red.Is().Defined();
    }

    [Fact]
    public void Defined_rejects_undefined_value() {
        Assert.Throws<AssertException>(() => { ((Color)99).Is().Defined(); });
    }

    [Fact]
    public void HaveFlag_checks_flag() {
        (Perm.Read | Perm.Write).Is().Flagged(Perm.Read);
    }

    [Fact]
    public void AsEnum_parses_name() {
        Assert.Equal(Color.Red, "Red".Is().AsEnum<Color>());
    }

    [Fact]
    public void Enum_entry_chains_with_defined() {
        "Red".Is().Enum<Color>().Defined();
    }

    [Fact]
    public void AsEnum_from_int_validates_defined() {
        Assert.Equal(Color.Blue, 2.Is().AsEnum<Color>());
    }

    [Fact]
    public void Enum_from_undefined_int_throws() {
        Assert.Throws<AssertException>(() => { 99.Is().Enum<Color>(); });
    }

    [Fact]
    public void AsEnum_invalid_name_throws() {
        Assert.Throws<AssertException>(() => { "nope".Is().AsEnum<Color>(); });
    }

    [Fact]
    public void AsEnum_invalid_name_guard_throws_argument() {
        Assert.Throws<ArgumentException>(() => { "nope".Guard().AsGuardEnum<Color>(); });
    }
}
