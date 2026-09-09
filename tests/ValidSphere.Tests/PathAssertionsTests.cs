using ValidSphere;

namespace ValidSphere.Tests;

public sealed class PathAssertionsTests {
    private static string NewRoot() {
        var root = Path.Combine(Path.GetTempPath(), "ValidSphereTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static string WriteFile(string root, string name, string content = "hello") {
        var path = Path.Combine(root, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void File_refines_path() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            var file = path.Is().File();
            Assert.EndsWith("a.txt", file.Value);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_refines_path() {
        var root = NewRoot();
        try {
            var dir = root.Is().Directory();
            Assert.EndsWith(Path.GetFileName(root), dir.Value);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Exists_accepts_present_file() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().Exists();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Exists_rejects_missing_file() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().Exists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Exists_guard_rejects_missing_file() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().Exists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Exists_accepts_present() {
        var root = NewRoot();
        try {
            root.Is().Directory().Exists();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Exists_rejects_missing() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing");
            Assert.Throws<AssertException>(() => { path.Is().Directory().Exists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void NotExists_accepts_missing_file() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing.txt");
            path.Is().File().NotExists();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void NotExists_rejects_present_file() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().NotExists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void NotExists_guard_rejects_present_file() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().NotExists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotExists_accepts_missing() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing");
            path.Is().Directory().NotExists();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotExists_rejects_present() {
        var root = NewRoot();
        try {
            Assert.Throws<AssertException>(() => { root.Is().Directory().NotExists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveExtension_accepts_match() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().HaveExtension(".txt");
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveExtension_accepts_ignoring_case() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().HaveExtension(".TXT", StringComparison.OrdinalIgnoreCase);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveExtension_accepts_array() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().HaveExtension([".md", ".txt"]);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveExtension_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().HaveExtension(".md"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveExtension_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().HaveExtension(".md"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveFileName_accepts_match() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().HaveFileName("a.txt");
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveFileName_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().HaveFileName("b.txt"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveFileName_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().HaveFileName("b.txt"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Absolute_accepts_absolute() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().Absolute();
            root.Is().Directory().Absolute();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Absolute_rejects_relative() {
        Assert.Throws<AssertException>(() => { "a.txt".Is().File().Absolute(); });
    }

    [Fact]
    public void Absolute_guard_rejects_relative() {
        Assert.ThrowsAny<ArgumentException>(() => { "a.txt".Guard().File().Absolute(); });
    }

    [Fact]
    public void NoExtension_accepts_extensionless() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a");
            path.Is().File().NoExtension();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void NoExtension_rejects_extension() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().NoExtension(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void NoExtension_guard_rejects_extension() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().NoExtension(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveName_accepts_match() {
        var root = NewRoot();
        try {
            var sub = Path.Combine(root, "sub");
            Directory.CreateDirectory(sub);
            sub.Is().Directory().HaveName("sub");
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveName_rejects_mismatch() {
        var root = NewRoot();
        try {
            Assert.Throws<AssertException>(() => { root.Is().Directory().HaveName("other"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void HaveName_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().HaveName("other"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_Empty_accepts_empty() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt", "");
            path.Is().File().Empty();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_Empty_rejects_content() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().Empty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_NotEmpty_accepts_content() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().NotEmpty();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_NotEmpty_rejects_empty() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt", "");
            Assert.Throws<AssertException>(() => { path.Is().File().NotEmpty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_NotEmpty_guard_rejects_empty() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt", "");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().NotEmpty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_Length_accepts_exact() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().Length(5);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_Length_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().Length(3); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_Length_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().Length(3); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_MinMaxRange_accept() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().MinLength(1).MaxLength(9).LengthInRange(1, 9);
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_MinMaxRange_reject() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().MinLength(9); });
            Assert.Throws<AssertException>(() => { path.Is().File().MaxLength(1); });
            Assert.Throws<AssertException>(() => { path.Is().File().LengthInRange(9, 19); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Empty_accepts_empty() {
        var root = NewRoot();
        try {
            var sub = Path.Combine(root, "sub");
            Directory.CreateDirectory(sub);
            sub.Is().Directory().Empty();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Empty_rejects_content() {
        var root = NewRoot();
        try {
            WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { root.Is().Directory().Empty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotEmpty_accepts_content() {
        var root = NewRoot();
        try {
            WriteFile(root, "a.txt");
            root.Is().Directory().NotEmpty();
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotEmpty_rejects_empty() {
        var root = NewRoot();
        try {
            var sub = Path.Combine(root, "sub");
            Directory.CreateDirectory(sub);
            Assert.Throws<AssertException>(() => { sub.Is().Directory().NotEmpty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotEmpty_guard_rejects_empty() {
        var root = NewRoot();
        try {
            var sub = Path.Combine(root, "sub");
            Directory.CreateDirectory(sub);
            Assert.ThrowsAny<ArgumentException>(() => { sub.Guard().Directory().NotEmpty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsFile_accepts_present() {
        var root = NewRoot();
        try {
            WriteFile(root, "a.txt");
            root.Is().Directory().ContainsFile("a.txt");
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsFile_rejects_missing() {
        var root = NewRoot();
        try {
            Assert.Throws<AssertException>(() => { root.Is().Directory().ContainsFile("missing.txt"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsFile_guard_rejects_missing() {
        var root = NewRoot();
        try {
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().ContainsFile("missing.txt"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsDirectory_accepts_present() {
        var root = NewRoot();
        try {
            Directory.CreateDirectory(Path.Combine(root, "sub"));
            root.Is().Directory().ContainsDirectory("sub");
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsDirectory_rejects_missing() {
        var root = NewRoot();
        try {
            Assert.Throws<AssertException>(() => { root.Is().Directory().ContainsDirectory("missing"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void ContainsDirectory_guard_rejects_missing() {
        var root = NewRoot();
        try {
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().ContainsDirectory("missing"); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_HaveFullPath_accepts_match() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            path.Is().File().HaveFullPath(Path.GetFullPath(path));
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_HaveFullPath_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.Throws<AssertException>(() => { path.Is().File().HaveFullPath(Path.Combine(root, "other.txt")); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_HaveFullPath_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().HaveFullPath(Path.Combine(root, "other.txt")); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_HaveFullPath_accepts_match() {
        var root = NewRoot();
        try {
            root.Is().Directory().HaveFullPath(Path.GetFullPath(root));
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_HaveFullPath_rejects_mismatch() {
        var root = NewRoot();
        try {
            Assert.Throws<AssertException>(() => { root.Is().Directory().HaveFullPath(Path.Combine(root, "other")); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_HaveFullPath_guard_rejects_mismatch() {
        var root = NewRoot();
        try {
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().HaveFullPath(Path.Combine(root, "other")); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Custom_message_surfaces() {
        var root = NewRoot();
        try {
            var path = Path.Combine(root, "missing.txt");
            var ex = Assert.Throws<AssertException>(() => { path.Is().File().Exists("custom"); });
            Assert.Contains("custom", ex.Message);
        } finally {
            Directory.Delete(root, true);
        }
    }
    [Fact]
    public void File_Empty_guard_rejects_content() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().Empty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void File_MinMaxRange_guard_rejects() {
        var root = NewRoot();
        try {
            var path = WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { path.Guard().File().MinLength(9); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Empty_guard_rejects_content() {
        var root = NewRoot();
        try {
            WriteFile(root, "a.txt");
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().Empty(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_NotExists_guard_rejects_present() {
        var root = NewRoot();
        try {
            Assert.ThrowsAny<ArgumentException>(() => { root.Guard().Directory().NotExists(); });
        } finally {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Directory_Absolute_rejects_relative() {
        Assert.Throws<AssertException>(() => { "sub".Is().Directory().Absolute(); });
    }

    [Fact]
    public void Directory_Absolute_guard_rejects_relative() {
        Assert.ThrowsAny<ArgumentException>(() => { "sub".Guard().Directory().Absolute(); });
    }

}
