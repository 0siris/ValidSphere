using ValidSphere;

namespace ValidSphere.Tests;

public sealed class ComparisonAssertionsTests {
    [Fact]
    public void Greater_accepts_greater() {
        5.Is().Greater(3);
    }

    [Fact]
    public void Greater_rejects_equal() {
        Assert.Throws<AssertException>(() => { 3.Is().Greater(3); });
    }

    [Fact]
    public void Greater_guard_rejects_smaller() {
        Assert.ThrowsAny<ArgumentException>(() => { 2.Guard().Greater(3); });
    }

    [Fact]
    public void GreaterEq_accepts_equal() {
        3.Is().GreaterEq(3);
    }

    [Fact]
    public void GreaterEq_rejects_smaller() {
        Assert.Throws<AssertException>(() => { 2.Is().GreaterEq(3); });
    }

    [Fact]
    public void GreaterEq_guard_rejects_smaller() {
        Assert.ThrowsAny<ArgumentException>(() => { 2.Guard().GreaterEq(3); });
    }

    [Fact]
    public void Less_accepts_smaller() {
        3.Is().Less(5);
    }

    [Fact]
    public void Less_rejects_equal() {
        Assert.Throws<AssertException>(() => { 3.Is().Less(3); });
    }

    [Fact]
    public void Less_guard_rejects_greater() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().Less(3); });
    }

    [Fact]
    public void LessEq_accepts_equal() {
        3.Is().LessEq(3);
    }

    [Fact]
    public void LessEq_rejects_greater() {
        Assert.Throws<AssertException>(() => { 5.Is().LessEq(3); });
    }

    [Fact]
    public void LessEq_guard_rejects_greater() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().LessEq(3); });
    }

    [Fact]
    public void Range_accepts_inside() {
        5.Is().Range(1, 10);
    }

    [Fact]
    public void Range_accepts_boundaries() {
        1.Is().Range(1, 10);
        10.Is().Range(1, 10);
    }

    [Fact]
    public void Range_rejects_outside() {
        Assert.Throws<AssertException>(() => { 11.Is().Range(1, 10); });
    }

    [Fact]
    public void Range_guard_rejects_outside() {
        Assert.ThrowsAny<ArgumentException>(() => { 0.Guard().Range(1, 10); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { 3.Is().Greater(3, _ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        5.Is().Greater(3, _ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { 3.Is().Greater(3, "custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
