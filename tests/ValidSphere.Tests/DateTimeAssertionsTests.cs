using ValidSphere;

namespace ValidSphere.Tests;

public sealed class DateTimeAssertionsTests {
    private static readonly DateTime Base = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Kind_accepts_match() {
        DateTime.SpecifyKind(Base, DateTimeKind.Utc).Is().Kind(DateTimeKind.Utc);
    }

    [Fact]
    public void Kind_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { Base.Is().Kind(DateTimeKind.Local); });
    }

    [Fact]
    public void Kind_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { Base.Guard().Kind(DateTimeKind.Local); });
    }

    [Fact]
    public void Utc_accepts_utc() {
        Base.Is().Utc();
    }

    [Fact]
    public void Utc_rejects_local() {
        var local = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Local);
        Assert.Throws<AssertException>(() => { local.Is().Utc(); });
    }

    [Fact]
    public void Utc_guard_rejects_local() {
        var local = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Local);
        Assert.ThrowsAny<ArgumentException>(() => { local.Guard().Utc(); });
    }

    [Fact]
    public void After_accepts_later() {
        Base.Is().After(Base.AddHours(-1));
    }

    [Fact]
    public void After_rejects_equal() {
        Assert.Throws<AssertException>(() => { Base.Is().After(Base); });
    }

    [Fact]
    public void After_guard_rejects_earlier() {
        Assert.ThrowsAny<ArgumentException>(() => { Base.Guard().After(Base.AddHours(1)); });
    }

    [Fact]
    public void NotBefore_accepts_equal() {
        Base.Is().NotBefore(Base);
    }

    [Fact]
    public void NotBefore_rejects_earlier() {
        Assert.Throws<AssertException>(() => { Base.Is().NotBefore(Base.AddHours(1)); });
    }

    [Fact]
    public void NotBefore_guard_rejects_earlier() {
        Assert.ThrowsAny<ArgumentException>(() => { Base.Guard().NotBefore(Base.AddHours(1)); });
    }

    [Fact]
    public void Before_accepts_earlier() {
        Base.Is().Before(Base.AddHours(1));
    }

    [Fact]
    public void Before_rejects_equal() {
        Assert.Throws<AssertException>(() => { Base.Is().Before(Base); });
    }

    [Fact]
    public void Before_guard_rejects_later() {
        Assert.ThrowsAny<ArgumentException>(() => { Base.Guard().Before(Base.AddHours(-1)); });
    }

    [Fact]
    public void NotAfter_accepts_equal() {
        Base.Is().NotAfter(Base);
    }

    [Fact]
    public void NotAfter_rejects_later() {
        Assert.Throws<AssertException>(() => { Base.Is().NotAfter(Base.AddHours(-1)); });
    }

    [Fact]
    public void NotAfter_guard_rejects_later() {
        Assert.ThrowsAny<ArgumentException>(() => { Base.Guard().NotAfter(Base.AddHours(-1)); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { Base.Is().Kind(DateTimeKind.Local, "custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
