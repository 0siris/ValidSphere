using ValidSphere;

namespace ValidSphere.Tests;

public sealed class CollectionAssertionsTests {
    [Fact]
    public void NotEmpty_accepts_nonempty() {
        new List<int> { 1 }.Is().NotNull().NotEmpty();
    }

    [Fact]
    public void NotEmpty_rejects_empty() {
        Assert.Throws<AssertException>(() => { new List<int>().Is().NotNull().NotEmpty(); });
    }

    [Fact]
    public void NotEmpty_guard_rejects_empty() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int>().Guard().NotNull().NotEmpty(); });
    }

    [Fact]
    public void Count_accepts_exact() {
        new List<int> { 1, 2 }.Is().NotNull().Count(2);
    }

    [Fact]
    public void Count_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().Count(2); });
    }

    [Fact]
    public void Count_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int> { 1 }.Guard().NotNull().Count(2); });
    }

    [Fact]
    public void MinCount_accepts_enough() {
        new List<int> { 1, 2 }.Is().NotNull().MinCount(1);
    }

    [Fact]
    public void MinCount_rejects_too_few() {
        Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().MinCount(2); });
    }

    [Fact]
    public void MinCount_guard_rejects_too_few() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int> { 1 }.Guard().NotNull().MinCount(2); });
    }

    [Fact]
    public void MaxCount_accepts_few_enough() {
        new List<int> { 1 }.Is().NotNull().MaxCount(2);
    }

    [Fact]
    public void MaxCount_rejects_too_many() {
        Assert.Throws<AssertException>(() => { new List<int> { 1, 2, 3 }.Is().NotNull().MaxCount(2); });
    }

    [Fact]
    public void MaxCount_guard_rejects_too_many() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int> { 1, 2, 3 }.Guard().NotNull().MaxCount(2); });
    }

    [Fact]
    public void CountInRange_accepts_inside() {
        new List<int> { 1, 2 }.Is().NotNull().CountInRange(1, 3);
    }

    [Fact]
    public void CountInRange_rejects_outside() {
        Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().CountInRange(2, 3); });
    }

    [Fact]
    public void CountInRange_guard_rejects_outside() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int> { 1 }.Guard().NotNull().CountInRange(2, 3); });
    }

    [Fact]
    public void Array_Length_accepts_exact() {
        new int[3].Is().NotNull().Length(3);
    }

    [Fact]
    public void Array_Length_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { new int[3].Is().NotNull().Length(5); });
    }

    [Fact]
    public void Array_Length_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { new int[3].Guard().NotNull().Length(5); });
    }

    [Fact]
    public void Array_MinMaxRange_accept() {
        new int[3].Is().NotNull().MinLength(1).MaxLength(5).LengthInRange(1, 5);
    }

    [Fact]
    public void Array_MinMaxRange_reject() {
        Assert.Throws<AssertException>(() => { new int[3].Is().NotNull().MinLength(5); });
        Assert.Throws<AssertException>(() => { new int[3].Is().NotNull().MaxLength(1); });
        Assert.Throws<AssertException>(() => { new int[3].Is().NotNull().LengthInRange(5, 9); });
    }

    [Fact]
    public void HashSet_Count_accepts_exact() {
        new HashSet<int> { 1, 2 }.Is().Count(2);
    }

    [Fact]
    public void HashSet_Count_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { new HashSet<int> { 1 }.Is().Count(2); });
    }

    [Fact]
    public void HashSet_Count_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { new HashSet<int> { 1 }.Guard().Count(2); });
    }

    [Fact]
    public void HashSet_NotEmpty_MinMaxRange_accept() {
        new HashSet<int> { 1, 2 }.Is().NotEmpty().MinCount(1).MaxCount(3).CountInRange(1, 3);
    }

    [Fact]
    public void HashSet_NotEmpty_rejects_empty() {
        Assert.Throws<AssertException>(() => { new HashSet<int>().Is().NotEmpty(); });
    }

    [Fact]
    public void SortedSet_Count_accepts_exact() {
        new SortedSet<string> { "a" }.Is().Count(1);
    }

    [Fact]
    public void SortedSet_Count_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { new SortedSet<string> { "a" }.Is().Count(2); });
    }

    [Fact]
    public void SortedSet_NotEmpty_MinMaxRange_accept() {
        new SortedSet<string> { "a", "b" }.Is().NotEmpty().MinCount(1).MaxCount(3).CountInRange(1, 3);
    }

    [Fact]
    public void ReadOnlyCollection_Count_accepts_exact() {
        IReadOnlyCollection<int> col = new List<int> { 1, 2 };
        col.Is().Count(2);
        col.Is().NotEmpty().MinCount(1).MaxCount(3).CountInRange(1, 3);
    }

    [Fact]
    public void Contains_accepts_member() {
        new List<int> { 1, 2 }.Is().NotNull().Contains(2);
        new HashSet<int> { 1, 2 }.Is().Contains(2);
    }

    [Fact]
    public void Contains_rejects_missing() {
        Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().Contains(2); });
    }

    [Fact]
    public void Contains_guard_rejects_missing() {
        Assert.ThrowsAny<ArgumentException>(() => { new List<int> { 1 }.Guard().NotNull().Contains(2); });
    }

    [Fact]
    public void MessageFactory_builds_message_lazily() {
        var ex = Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().Count(2, _ => "lazy"); });
        Assert.Contains("lazy", ex.Message);

        var invoked = false;
        new List<int> { 1, 2 }.Is().NotNull().Count(2, _ => { invoked = true; return "lazy"; });
        Assert.False(invoked);
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { new List<int> { 1 }.Is().NotNull().Count(2, "custom"); });
        Assert.Contains("custom", ex.Message);
    }
    [Fact]
    public void Array_MinMaxRange_guard_rejects() {
        Assert.ThrowsAny<ArgumentException>(() => { new int[3].Guard().NotNull().MinLength(5); });
    }

    [Fact]
    public void HashSet_MinMaxRange_reject() {
        Assert.Throws<AssertException>(() => { new HashSet<int>().Is().MinCount(1); });
        Assert.Throws<AssertException>(() => { new HashSet<int> { 1, 2, 3 }.Is().MaxCount(1); });
        Assert.Throws<AssertException>(() => { new HashSet<int> { 1 }.Is().CountInRange(2, 3); });
    }

    [Fact]
    public void HashSet_MinMaxRange_guard_rejects() {
        Assert.ThrowsAny<ArgumentException>(() => { new HashSet<int>().Guard().MinCount(1); });
    }

    [Fact]
    public void SortedSet_NotEmpty_rejects_empty() {
        Assert.Throws<AssertException>(() => { new SortedSet<string>().Is().NotEmpty(); });
    }

    [Fact]
    public void SortedSet_Count_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { new SortedSet<string> { "a" }.Guard().Count(2); });
    }

    [Fact]
    public void SortedSet_MinMaxRange_reject() {
        Assert.Throws<AssertException>(() => { new SortedSet<string>().Is().MinCount(1); });
        Assert.Throws<AssertException>(() => { new SortedSet<string> { "a", "b", "c" }.Is().MaxCount(1); });
        Assert.Throws<AssertException>(() => { new SortedSet<string> { "a" }.Is().CountInRange(2, 3); });
    }

}
