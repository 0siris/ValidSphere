using ValidSphere;

namespace ValidSphere.Tests;

public sealed class UriAssertionsTests {
    private const string AbsoluteUrl = "http://example.com/path";

    [Fact]
    public void Uri_parses_and_refines() {
        Uri uri = AbsoluteUrl.Is().Uri();
        Assert.Equal("example.com", uri.Host);
        AbsoluteUrl.Is().Uri().Absolute();
    }

    [Fact]
    public void Uri_rejects_invalid() {
        Assert.Throws<AssertException>(() => { "nope".Is().Uri(); });
    }

    [Fact]
    public void Uri_guard_rejects_invalid() {
        Assert.ThrowsAny<ArgumentException>(() => { "nope".Guard().Uri(); });
    }

    [Fact]
    public void Absolute_accepts_absolute() {
        AbsoluteUrl.Is().Uri().Absolute();
    }

    [Fact]
    public void Absolute_rejects_relative() {
        Assert.Throws<AssertException>(() => { "a/b".Is().Uri(UriKind.RelativeOrAbsolute).Absolute(); });
    }

    [Fact]
    public void Absolute_guard_rejects_relative() {
        Assert.ThrowsAny<ArgumentException>(() => { "a/b".Guard().Uri(UriKind.RelativeOrAbsolute).Absolute(); });
    }

    [Fact]
    public void HaveScheme_accepts_match() {
        AbsoluteUrl.Is().Uri().HaveScheme("http");
    }

    [Fact]
    public void HaveScheme_accepts_ignoring_case() {
        AbsoluteUrl.Is().Uri().HaveScheme("HTTP", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HaveScheme_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { AbsoluteUrl.Is().Uri().HaveScheme("https"); });
    }

    [Fact]
    public void HaveScheme_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { AbsoluteUrl.Guard().Uri().HaveScheme("https"); });
    }

    [Fact]
    public void HaveHost_accepts_match() {
        AbsoluteUrl.Is().Uri().HaveHost("example.com");
    }

    [Fact]
    public void HaveHost_accepts_ignoring_case() {
        AbsoluteUrl.Is().Uri().HaveHost("EXAMPLE.COM", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HaveHost_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { AbsoluteUrl.Is().Uri().HaveHost("other.com"); });
    }

    [Fact]
    public void HaveHost_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { AbsoluteUrl.Guard().Uri().HaveHost("other.com"); });
    }

    [Fact]
    public void HavePort_accepts_match() {
        "http://example.com:8080/".Is().Uri().HavePort(8080);
    }

    [Fact]
    public void HavePort_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { "http://example.com:8080/".Is().Uri().HavePort(9090); });
    }

    [Fact]
    public void HavePort_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { "http://example.com:8080/".Guard().Uri().HavePort(9090); });
    }

    [Fact]
    public void Loopback_accepts_localhost() {
        "http://localhost/".Is().Uri().Loopback();
    }

    [Fact]
    public void Loopback_rejects_remote() {
        Assert.Throws<AssertException>(() => { AbsoluteUrl.Is().Uri().Loopback(); });
    }

    [Fact]
    public void Loopback_guard_rejects_remote() {
        Assert.ThrowsAny<ArgumentException>(() => { AbsoluteUrl.Guard().Uri().Loopback(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { "nope".Is().Uri(message: "custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
