using ValidSphere;

namespace ValidSphere.Tests;

public sealed class DateTimeOffsetAssertionsTests {
    [Fact]
    public void Utc_accepts_zero_offset() {
        new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero).Is().Utc();
    }

    [Fact]
    public void Utc_rejects_offset() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        Assert.Throws<AssertException>(() => { value.Is().Utc(); });
    }

    [Fact]
    public void Utc_guard_rejects_offset() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().Utc(); });
    }

    [Fact]
    public void HaveOffset_accepts_match() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        value.Is().HaveOffset(TimeSpan.FromHours(2));
    }

    [Fact]
    public void HaveOffset_rejects_mismatch() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        Assert.Throws<AssertException>(() => { value.Is().HaveOffset(TimeSpan.FromHours(1)); });
    }

    [Fact]
    public void HaveOffset_guard_rejects_mismatch() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        Assert.ThrowsAny<ArgumentException>(() => { value.Guard().HaveOffset(TimeSpan.FromHours(1)); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var value = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));
        var ex = Assert.Throws<AssertException>(() => { value.Is().Utc("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
