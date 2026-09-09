using ValidSphere;

namespace ValidSphere.Tests;

public sealed class CharAssertionsTests {
    [Fact]
    public void IsLetter_accepts_letter() {
        'a'.Is().IsLetter();
    }

    [Fact]
    public void IsLetter_rejects_digit() {
        Assert.Throws<AssertException>(() => { '5'.Is().IsLetter(); });
    }

    [Fact]
    public void IsLetter_guard_rejects_digit() {
        Assert.ThrowsAny<ArgumentException>(() => { '5'.Guard().IsLetter(); });
    }

    [Fact]
    public void IsDigit_accepts_digit() {
        '5'.Is().IsDigit();
    }

    [Fact]
    public void IsDigit_rejects_letter() {
        Assert.Throws<AssertException>(() => { 'a'.Is().IsDigit(); });
    }

    [Fact]
    public void IsDigit_guard_rejects_letter() {
        Assert.ThrowsAny<ArgumentException>(() => { 'a'.Guard().IsDigit(); });
    }

    [Fact]
    public void IsWhiteSpace_accepts_space() {
        ' '.Is().IsWhiteSpace();
    }

    [Fact]
    public void IsWhiteSpace_rejects_letter() {
        Assert.Throws<AssertException>(() => { 'a'.Is().IsWhiteSpace(); });
    }

    [Fact]
    public void IsWhiteSpace_guard_rejects_letter() {
        Assert.ThrowsAny<ArgumentException>(() => { 'a'.Guard().IsWhiteSpace(); });
    }

    [Fact]
    public void IsUpper_accepts_uppercase() {
        'A'.Is().IsUpper();
    }

    [Fact]
    public void IsUpper_rejects_lowercase() {
        Assert.Throws<AssertException>(() => { 'a'.Is().IsUpper(); });
    }

    [Fact]
    public void IsUpper_guard_rejects_lowercase() {
        Assert.ThrowsAny<ArgumentException>(() => { 'a'.Guard().IsUpper(); });
    }

    [Fact]
    public void IsLower_accepts_lowercase() {
        'a'.Is().IsLower();
    }

    [Fact]
    public void IsLower_rejects_uppercase() {
        Assert.Throws<AssertException>(() => { 'A'.Is().IsLower(); });
    }

    [Fact]
    public void IsLower_guard_rejects_uppercase() {
        Assert.ThrowsAny<ArgumentException>(() => { 'A'.Guard().IsLower(); });
    }

    [Fact]
    public void Custom_message_surfaces() {
        var ex = Assert.Throws<AssertException>(() => { 'a'.Is().IsUpper("custom"); });
        Assert.Contains("custom", ex.Message);
    }
}
