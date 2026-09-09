using System.Net;
using ValidSphere;

namespace ValidSphere.Tests;

public sealed class IpAssertionsTests {
    [Fact]
    public void Loopback_accepts_ipv4_loopback() {
        IPAddress.Loopback.Is().Loopback();
    }

    [Fact]
    public void Loopback_accepts_ipv6_loopback() {
        IPAddress.IPv6Loopback.Is().Loopback();
    }

    [Fact]
    public void Loopback_rejects_remote() {
        Assert.Throws<AssertException>(() => { IPAddress.Parse("8.8.8.8").Is().Loopback(); });
    }

    [Fact]
    public void Loopback_guard_rejects_remote() {
        Assert.ThrowsAny<ArgumentException>(() => { IPAddress.Parse("8.8.8.8").Guard().Loopback(); });
    }

    [Fact]
    public void IPv4_accepts_ipv4() {
        IPAddress.Loopback.Is().IPv4();
    }

    [Fact]
    public void IPv4_rejects_ipv6() {
        Assert.Throws<AssertException>(() => { IPAddress.IPv6Loopback.Is().IPv4(); });
    }

    [Fact]
    public void IPv4_guard_rejects_ipv6() {
        Assert.ThrowsAny<ArgumentException>(() => { IPAddress.IPv6Loopback.Guard().IPv4(); });
    }

    [Fact]
    public void IPv6_accepts_ipv6() {
        IPAddress.IPv6Loopback.Is().IPv6();
    }

    [Fact]
    public void IPv6_rejects_ipv4() {
        Assert.Throws<AssertException>(() => { IPAddress.Loopback.Is().IPv6(); });
    }

    [Fact]
    public void IPv6_guard_rejects_ipv4() {
        Assert.ThrowsAny<ArgumentException>(() => { IPAddress.Loopback.Guard().IPv6(); });
    }

    [Fact]
    public void HavePort_accepts_match() {
        new IPEndPoint(IPAddress.Loopback, 8080).Is().HavePort(8080);
    }

    [Fact]
    public void HavePort_rejects_mismatch() {
        Assert.Throws<AssertException>(() => { new IPEndPoint(IPAddress.Loopback, 8080).Is().HavePort(9090); });
    }

    [Fact]
    public void HavePort_guard_rejects_mismatch() {
        Assert.ThrowsAny<ArgumentException>(() => { new IPEndPoint(IPAddress.Loopback, 8080).Guard().HavePort(9090); });
    }

    [Fact]
    public void EndPoint_Loopback_accepts_loopback() {
        new IPEndPoint(IPAddress.Loopback, 8080).Is().Loopback();
    }

    [Fact]
    public void EndPoint_Loopback_rejects_remote() {
        var remote = new IPEndPoint(IPAddress.Parse("8.8.8.8"), 8080);
        Assert.Throws<AssertException>(() => { remote.Is().Loopback(); });
    }

    [Fact]
    public void EndPoint_Loopback_guard_rejects_remote() {
        var remote = new IPEndPoint(IPAddress.Parse("8.8.8.8"), 8080);
        Assert.ThrowsAny<ArgumentException>(() => { remote.Guard().Loopback(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { IPAddress.Loopback.Is().IPv6("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
