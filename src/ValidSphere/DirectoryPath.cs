using System.IO;

namespace ValidSphere;

/// <summary>
///     Represents a file-system path in directory mode, refining a <see cref="string" /> into the directory domain.
/// </summary>
/// <remarks>
///     The struct carries the raw path and forwards a curated set of members to the static <see cref="Directory" /> API,
///     so assertions and I/O can be combined in one fluent chain: <c>path.Is().Directory().Exists()</c>, <c>dir.GetFiles()</c>.
/// </remarks>
public readonly struct DirectoryPath {
    /// <summary>
    ///     Initializes a new instance of the <see cref="DirectoryPath" /> struct.
    /// </summary>
    /// <param name="value">The directory path. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
    public DirectoryPath(string value) {
        ThrowHelper.ThrowIfNull(value, nameof(value));
        Value = value;
    }

    /// <summary>
    ///     Gets the underlying directory path.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets a value indicating whether the directory exists.
    /// </summary>
    public bool Exists => Directory.Exists(Value);

    /// <summary>
    ///     Creates the directory and any missing intermediate directories.
    /// </summary>
    public void Create() => _ = Directory.CreateDirectory(Value);

    /// <summary>
    ///     Deletes the directory.
    /// </summary>
    /// <param name="recursive">
    ///     <see langword="true" /> to delete the directory and its entire subtree; otherwise <see langword="false" />.
    /// </param>
    public void Delete(bool recursive = false) => Directory.Delete(Value, recursive);

    /// <summary>
    ///     Gets the full file paths of all files in the directory matching the pattern.
    /// </summary>
    /// <param name="pattern">The file name pattern to match. Defaults to all files.</param>
    public string[] GetFiles(string pattern = "*") => Directory.GetFiles(Value, pattern);

    /// <summary>
    ///     Gets the full paths of all subdirectories in the directory matching the pattern.
    /// </summary>
    /// <param name="pattern">The directory name pattern to match. Defaults to all directories.</param>
    public string[] GetDirectories(string pattern = "*") => Directory.GetDirectories(Value, pattern);

    /// <summary>
    ///     Creates a subdirectory and returns it as a <see cref="DirectoryPath" />.
    /// </summary>
    /// <param name="name">The subdirectory name.</param>
    public DirectoryPath CreateSubdirectory(string name) {
        var combined = Path.Combine(Value, name);
        Directory.CreateDirectory(combined);
        return new DirectoryPath(combined);
    }

    /// <summary>
    ///     Combines the directory with a file name into a <see cref="FilePath" />, without touching the file system.
    /// </summary>
    /// <param name="fileName">The file name to combine.</param>
    public FilePath CombineFile(string fileName) => new(Path.Combine(Value, fileName));

    /// <summary>
    ///     Converts this <see cref="DirectoryPath" /> to its underlying path string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    ///     Converts a <see cref="DirectoryPath" /> implicitly to its underlying path string.
    /// </summary>
    public static implicit operator string(DirectoryPath path) => path.Value;
}
