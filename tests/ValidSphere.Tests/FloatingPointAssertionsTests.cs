using ValidSphere;

namespace ValidSphere.Tests;

public sealed class FloatingPointAssertionsTests {
    [Fact]
    public void Approx_accepts_within_tolerance() {
        1.0.Is().Approx(1.05, 0.1);
    }

    [Fact]
    public void Approx_rejects_outside_tolerance() {
        Assert.Throws<AssertException>(() => { 1.0.Is().Approx(2.0, 0.1); });
    }

    [Fact]
    public void Approx_guard_rejects_outside_tolerance() {
        Assert.ThrowsAny<ArgumentException>(() => { 1.0.Guard().Approx(2.0, 0.1); });
    }

    [Fact]
    public void NotNaN_accepts_number() {
        1.5.Is().NotNaN();
    }

    [Fact]
    public void NotNaN_rejects_nan() {
        Assert.Throws<AssertException>(() => { double.NaN.Is().NotNaN(); });
    }

    [Fact]
    public void NotNaN_guard_rejects_nan() {
        Assert.ThrowsAny<ArgumentException>(() => { double.NaN.Guard().NotNaN(); });
    }

    [Fact]
    public void Finite_accepts_number() {
        1.5.Is().Finite();
    }

    [Fact]
    public void Finite_rejects_infinity() {
        Assert.Throws<AssertException>(() => { double.PositiveInfinity.Is().Finite(); });
    }

    [Fact]
    public void Finite_guard_rejects_infinity() {
        Assert.ThrowsAny<ArgumentException>(() => { double.PositiveInfinity.Guard().Finite(); });
    }

    [Fact]
    public void Infinite_accepts_infinity() {
        double.PositiveInfinity.Is().Infinite();
    }

    [Fact]
    public void Infinite_rejects_number() {
        Assert.Throws<AssertException>(() => { 1.5.Is().Infinite(); });
    }

    [Fact]
    public void Infinite_guard_rejects_number() {
        Assert.ThrowsAny<ArgumentException>(() => { 1.5.Guard().Infinite(); });
    }

    [Fact]
    public void PositiveInfinity_accepts_positive_infinity() {
        double.PositiveInfinity.Is().PositiveInfinity();
    }

    [Fact]
    public void PositiveInfinity_rejects_negative_infinity() {
        Assert.Throws<AssertException>(() => { double.NegativeInfinity.Is().PositiveInfinity(); });
    }

    [Fact]
    public void PositiveInfinity_guard_rejects_number() {
        Assert.ThrowsAny<ArgumentException>(() => { 1.5.Guard().PositiveInfinity(); });
    }

    [Fact]
    public void NegativeInfinity_accepts_negative_infinity() {
        double.NegativeInfinity.Is().NegativeInfinity();
    }

    [Fact]
    public void NegativeInfinity_rejects_positive_infinity() {
        Assert.Throws<AssertException>(() => { double.PositiveInfinity.Is().NegativeInfinity(); });
    }

    [Fact]
    public void NegativeInfinity_guard_rejects_number() {
        Assert.ThrowsAny<ArgumentException>(() => { 1.5.Guard().NegativeInfinity(); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { 1.0.Is().Approx(2.0, 0.1, _ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        1.0.Is().Approx(1.05, 0.1, _ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { double.NaN.Is().NotNaN("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
