using ValidSphere;

namespace ValidSphere.Tests;

public sealed class NumericAssertionsTests {
    [Fact]
    public void Positive_accepts_positive() {
        5.Is().Positive();
    }

    [Fact]
    public void Positive_rejects_zero() {
        Assert.Throws<AssertException>(() => { 0.Is().Positive(); });
    }

    [Fact]
    public void Positive_guard_rejects_negative() {
        Assert.ThrowsAny<ArgumentException>(() => { (-3).Guard().Positive(); });
    }

    [Fact]
    public void NonNegative_accepts_zero() {
        0.Is().NonNegative();
    }

    [Fact]
    public void NonNegative_rejects_negative() {
        Assert.Throws<AssertException>(() => { (-1).Is().NonNegative(); });
    }

    [Fact]
    public void NonNegative_guard_rejects_negative() {
        Assert.ThrowsAny<ArgumentException>(() => { (-1).Guard().NonNegative(); });
    }

    [Fact]
    public void Negative_accepts_negative() {
        (-2).Is().Negative();
    }

    [Fact]
    public void Negative_rejects_zero() {
        Assert.Throws<AssertException>(() => { 0.Is().Negative(); });
    }

    [Fact]
    public void Negative_guard_rejects_positive() {
        Assert.ThrowsAny<ArgumentException>(() => { 2.Guard().Negative(); });
    }

    [Fact]
    public void Zero_accepts_zero() {
        0.Is().Zero();
    }

    [Fact]
    public void Zero_rejects_nonzero() {
        Assert.Throws<AssertException>(() => { 1.Is().Zero(); });
    }

    [Fact]
    public void Zero_guard_rejects_nonzero() {
        Assert.ThrowsAny<ArgumentException>(() => { 1.Guard().Zero(); });
    }

    [Fact]
    public void NonZero_accepts_nonzero() {
        1.Is().NonZero();
    }

    [Fact]
    public void NonZero_rejects_zero() {
        Assert.Throws<AssertException>(() => { 0.Is().NonZero(); });
    }

    [Fact]
    public void NonZero_guard_rejects_zero() {
        Assert.ThrowsAny<ArgumentException>(() => { 0.Guard().NonZero(); });
    }

    [Fact]
    public void Even_accepts_even() {
        4.Is().Even();
    }

    [Fact]
    public void Even_rejects_odd() {
        Assert.Throws<AssertException>(() => { 3.Is().Even(); });
    }

    [Fact]
    public void Even_guard_rejects_odd() {
        Assert.ThrowsAny<ArgumentException>(() => { 3.Guard().Even(); });
    }

    [Fact]
    public void Odd_accepts_odd() {
        3.Is().Odd();
    }

    [Fact]
    public void Odd_rejects_even() {
        Assert.Throws<AssertException>(() => { 4.Is().Odd(); });
    }

    [Fact]
    public void Odd_guard_rejects_even() {
        Assert.ThrowsAny<ArgumentException>(() => { 4.Guard().Odd(); });
    }

    [Fact]
    public void DivisibleBy_accepts_divisible() {
        6.Is().DivisibleBy(3);
    }

    [Fact]
    public void DivisibleBy_rejects_remainder() {
        Assert.Throws<AssertException>(() => { 7.Is().DivisibleBy(3); });
    }

    [Fact]
    public void DivisibleBy_guard_rejects_remainder() {
        Assert.ThrowsAny<ArgumentException>(() => { 7.Guard().DivisibleBy(3); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { 0.Is().Positive("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
