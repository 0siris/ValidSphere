using ValidSphere;

namespace ValidSphere.Tests;

public sealed class TimeSpanAssertionsTests {
    [Fact]
    public void Positive_accepts_positive() {
        TimeSpan.FromHours(1).Is().Positive();
    }

    [Fact]
    public void Positive_rejects_zero() {
        Assert.Throws<AssertException>(() => { TimeSpan.Zero.Is().Positive(); });
    }

    [Fact]
    public void Positive_guard_rejects_negative() {
        Assert.ThrowsAny<ArgumentException>(() => { TimeSpan.FromHours(-1).Guard().Positive(); });
    }

    [Fact]
    public void NonNegative_accepts_zero() {
        TimeSpan.Zero.Is().NonNegative();
    }

    [Fact]
    public void NonNegative_rejects_negative() {
        Assert.Throws<AssertException>(() => { TimeSpan.FromHours(-1).Is().NonNegative(); });
    }

    [Fact]
    public void NonNegative_guard_rejects_negative() {
        Assert.ThrowsAny<ArgumentException>(() => { TimeSpan.FromHours(-1).Guard().NonNegative(); });
    }

    [Fact]
    public void Zero_accepts_zero() {
        TimeSpan.Zero.Is().Zero();
    }

    [Fact]
    public void Zero_rejects_nonzero() {
        Assert.Throws<AssertException>(() => { TimeSpan.FromHours(1).Is().Zero(); });
    }

    [Fact]
    public void Zero_guard_rejects_nonzero() {
        Assert.ThrowsAny<ArgumentException>(() => { TimeSpan.FromHours(1).Guard().Zero(); });
    }

    [Fact]
    public void WithinTimeout_accepts_inside() {
        TimeSpan.FromSeconds(1).Is().WithinTimeout(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void WithinTimeout_rejects_overrun() {
        Assert.Throws<AssertException>(() => { TimeSpan.FromSeconds(10).Is().WithinTimeout(TimeSpan.FromSeconds(5)); });
    }

    [Fact]
    public void WithinTimeout_guard_rejects_overrun() {
        Assert.ThrowsAny<ArgumentException>(() => { TimeSpan.FromSeconds(10).Guard().WithinTimeout(TimeSpan.FromSeconds(5)); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { TimeSpan.Zero.Is().Positive("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
