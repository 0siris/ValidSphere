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
        "3f2504e0-4f89-11d3-9a0c-0305e82c3301"
            .Is()
            .Guid().NotEmpty();
    }

    [Fact]
    public void AsGuid_extracts_raw() {
        var sample = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
        Guid parsed = sample.Is().AsGuid();
        Assert.Equal(Guid.Parse(sample), parsed);
    }

    [Fact]
    public void Is_accepts_null_subject() {
        string? s = null;
        Assert.Throws<AssertException>(() => {
            s.Is()
            .NotNull();
        });
    }

    [Fact]
    public void Guard_accepts_null_subject() {
        string? s = null;
        Assert.Throws<ArgumentNullException>(() => { s.Guard().NotNull(); });
    }

    [Fact]
    public void Nullable_string_entry_refines() {
        string? name = " x ";
        ((string?)name).Is().NotNullOrWhiteSpace();
    }

    [Fact]
    public void Nullable_string_null_entry_fails() {
        string? name = null;
        Assert.Throws<AssertException>(() => { ((string?)name).Is().NotNullOrWhiteSpace(); });
    }

    [Fact]
    public void Nullable_guid_entry_refines() {
        Guid? id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
        Guid g = ((Guid?)id).Is().NotNull().Value;
        Assert.Equal(id.Value, g);
        Guid? n = null;
        Assert.Throws<AssertException>(() => { ((Guid?)n).Is().NotNull(); });
    }

    [Fact]
    public void Nullable_class_entry_refines_in_generic_context() {
        Assert.Equal("x", RequireNonNull("x"));
        Assert.Throws<AssertException>(() => { RequireNonNull<string>(null); });
    }

    private static T RequireNonNull<T>(T? value) where T : class
        => value.Is().NotNull().Value;

}