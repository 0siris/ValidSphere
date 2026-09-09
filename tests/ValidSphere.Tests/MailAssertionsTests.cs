using System.Net.Mail;
using ValidSphere;

namespace ValidSphere.Tests;

public sealed class MailAssertionsTests {
    [Fact]
    public void MailAddress_parses_and_refines() {
        MailAddress address = "a@b.com".Is().MailAddress();
        Assert.Equal("a@b.com", address.Address);
        Assert.Equal("b.com", address.Host);
    }

    [Fact]
    public void MailAddress_rejects_invalid() {
        Assert.Throws<AssertException>(() => { "nope".Is().MailAddress(); });
    }

    [Fact]
    public void MailAddress_guard_rejects_invalid() {
        Assert.ThrowsAny<ArgumentException>(() => { "nope".Guard().MailAddress(); });
    }

    [Fact]
    public void HaveHost_accepts_match() {
        "a@b.com".Is().MailAddress().HaveHost("b.com");
    }

    [Fact]
    public void HaveHost_accepts_ignoring_case() {
        "a@b.com".Is().MailAddress().HaveHost("B.COM", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HaveHost_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { "a@b.com".Is().MailAddress().HaveHost("x.com"); });
    }

    [Fact]
    public void HaveHost_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { "a@b.com".Guard().MailAddress().HaveHost("x.com"); });
    }

    [Fact]
    public void HaveUser_accepts_match() {
        "a@b.com".Is().MailAddress().HaveUser("a");
    }

    [Fact]
    public void HaveUser_accepts_ignoring_case() {
        "a@b.com".Is().MailAddress().HaveUser("A", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HaveUser_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { "a@b.com".Is().MailAddress().HaveUser("z"); });
    }

    [Fact]
    public void HaveUser_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { "a@b.com".Guard().MailAddress().HaveUser("z"); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { "nope".Is().MailAddress("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
