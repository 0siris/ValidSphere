using ValidSphere;

namespace ValidSphere.Tests;

public sealed class DateOnlyAssertionsTests {
    [Fact]
    public void Today_accepts_today() {
        DateOnly.FromDateTime(DateTime.Now).Is().Today();
    }

    [Fact]
    public void Today_rejects_other_day() {
        Assert.Throws<AssertException>(() => { new DateOnly(2000, 1, 1).Is().Today(); });
    }

    [Fact]
    public void Today_guard_rejects_other_day() {
        Assert.ThrowsAny<ArgumentException>(() => { new DateOnly(2000, 1, 1).Guard().Today(); });
    }

    [Fact]
    public void Weekday_accepts_weekday() {
        new DateOnly(2026, 9, 9).Is().Weekday();
    }

    [Fact]
    public void Weekday_rejects_weekend() {
        Assert.Throws<AssertException>(() => { new DateOnly(2026, 9, 12).Is().Weekday(); });
    }

    [Fact]
    public void Weekday_guard_rejects_weekend() {
        Assert.ThrowsAny<ArgumentException>(() => { new DateOnly(2026, 9, 12).Guard().Weekday(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { new DateOnly(2000, 1, 1).Is().Today("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
