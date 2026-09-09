using ValidSphere;

namespace ValidSphere.Tests;

public sealed class CoreAssertionsTests {
    [Fact]
    public void Eq_accepts_equal() {
        5.Is().Eq(5);
    }

    [Fact]
    public void Eq_rejects_unequal() {
        Assert.Throws<AssertException>(() => { 5.Is().Eq(6); });
    }

    [Fact]
    public void Eq_guard_rejects_unequal() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().Eq(6); });
    }

    [Fact]
    public void NotEq_accepts_unequal() {
        5.Is().NotEq(6);
    }

    [Fact]
    public void NotEq_rejects_equal() {
        Assert.Throws<AssertException>(() => { 5.Is().NotEq(5); });
    }

    [Fact]
    public void NotEq_guard_rejects_equal() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().NotEq(5); });
    }

    [Fact]
    public void True_accepts_true() {
        true.Is().True();
    }

    [Fact]
    public void True_rejects_false() {
        Assert.Throws<AssertException>(() => { false.Is().True(); });
    }

    [Fact]
    public void True_guard_rejects_false() {
        Assert.ThrowsAny<ArgumentException>(() => { false.Guard().True(); });
    }

    [Fact]
    public void False_accepts_false() {
        false.Is().False();
    }

    [Fact]
    public void False_rejects_true() {
        Assert.Throws<AssertException>(() => { true.Is().False(); });
    }

    [Fact]
    public void False_guard_rejects_true() {
        Assert.ThrowsAny<ArgumentException>(() => { true.Guard().False(); });
    }

    [Fact]
    public void Satisfy_condition_accepts_true() {
        5.Is().Satisfy(true);
    }

    [Fact]
    public void Satisfy_condition_rejects_false() {
        Assert.Throws<AssertException>(() => { 5.Is().Satisfy(false); });
    }

    [Fact]
    public void Satisfy_condition_guard_rejects_false() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().Satisfy(false); });
    }

    [Fact]
    public void Satisfy_predicate_accepts_match() {
        5.Is().Satisfy(x => x > 0);
    }

    [Fact]
    public void Satisfy_predicate_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { 5.Is().Satisfy(x => x < 0); });
    }

    [Fact]
    public void Satisfy_predicate_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { 5.Guard().Satisfy(x => x < 0); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { 5.Is().Eq(6, _ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        5.Is().Eq(5, _ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { 5.Is().Eq(6, "custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
