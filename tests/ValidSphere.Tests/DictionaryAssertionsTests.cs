using ValidSphere;

namespace ValidSphere.Tests;

public sealed class DictionaryAssertionsTests {
    [Fact]
    public void ContainsKey_accepts_present() {
        new Dictionary<string, int> { ["a"] = 1 }.Is().NotNull().ContainsKey("a");
    }

    [Fact]
    public void ContainsKey_accepts_interface_shapes() {
        IDictionary<string, int> dict = new Dictionary<string, int> { ["a"] = 1 };
        dict.Is().ContainsKey("a");
        IReadOnlyDictionary<string, int> ro = new Dictionary<string, int> { ["a"] = 1 };
        ro.Is().ContainsKey("a");
    }

    [Fact]
    public void ContainsKey_rejects_missing() {
        Assert.Throws<AssertException>(() => { new Dictionary<string, int>().Is().NotNull().ContainsKey("a"); });
    }

    [Fact]
    public void ContainsKey_guard_rejects_missing() {
        Assert.ThrowsAny<ArgumentException>(() => { new Dictionary<string, int>().Guard().NotNull().ContainsKey("a"); });
    }

    [Fact]
    public void NotContainsKey_accepts_missing() {
        new Dictionary<string, int>().Is().NotNull().NotContainsKey("a");
    }

    [Fact]
    public void NotContainsKey_accepts_interface_shapes() {
        IDictionary<string, int> dict = new Dictionary<string, int>();
        dict.Is().NotContainsKey("a");
        IReadOnlyDictionary<string, int> ro = new Dictionary<string, int>();
        ro.Is().NotContainsKey("a");
    }

    [Fact]
    public void NotContainsKey_rejects_present() {
        Assert.Throws<AssertException>(() => { new Dictionary<string, int> { ["a"] = 1 }.Is().NotNull().NotContainsKey("a"); });
    }

    [Fact]
    public void NotContainsKey_guard_rejects_present() {
        Assert.ThrowsAny<ArgumentException>(() => { new Dictionary<string, int> { ["a"] = 1 }.Guard().NotNull().NotContainsKey("a"); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { new Dictionary<string, int>().Is().NotNull().ContainsKey("a", "custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
