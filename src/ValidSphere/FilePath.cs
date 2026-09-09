using System.IO;

namespace ValidSphere;

/// <summary>
///     Represents a file-system path in file mode, refining a <see cref="string" /> into the file domain.
/// </summary>
/// <remarks>
///     The struct carries the raw path and forwards a curated set of members to the static <see cref="File" /> API,
///     so assertions and I/O can be combined in one fluent chain: <c>path.Is().File().Exists()</c>, <c>file.ReadAllText()</c>.
/// </remarks>
public readonly struct FilePath {
    /// <summary>
    ///     Initializes a new instance of the <see cref="FilePath" /> struct.
    /// </summary>
    /// <param name="value">The file path. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
    public FilePath(string value) {
        ThrowHelper.ThrowIfNull(value, nameof(value));
        Value = value;
    }

    /// <summary>
    ///     Gets the underlying file path.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets a value indicating whether the file exists.
    /// </summary>
    public bool Exists => File.Exists(Value);

    /// <summary>
    ///     Gets the file name, including its extension.
    /// </summary>
    public string Name => Path.GetFileName(Value);

    /// <summary>
    ///     Gets the file extension, including the leading dot.
    /// </summary>
    public string Extension => Path.GetExtension(Value);

    /// <summary>
    ///     Gets the directory portion of the path, or <see langword="null" /> if the path has no directory component.
    /// </summary>
    public string? DirectoryName => Path.GetDirectoryName(Value);

    /// <summary>
    ///     Gets the length of the file in bytes.
    /// </summary>
    public long Length() => new FileInfo(Value).Length;

    /// <summary>
    ///     Reads all text from the file.
    /// </summary>
    public string ReadAllText() => File.ReadAllText(Value);

    /// <summary>
    ///     Reads all bytes from the file.
    /// </summary>
    public byte[] ReadAllBytes() => File.ReadAllBytes(Value);

    /// <summary>
    ///     Reads all lines from the file.
    /// </summary>
    public string[] ReadAllLines() => File.ReadAllLines(Value);

    /// <summary>
    ///     Writes the given text to the file, creating or overwriting it.
    /// </summary>
    public void WriteAllText(string contents) => File.WriteAllText(Value, contents);

    /// <summary>
    ///     Writes the given bytes to the file, creating or overwriting it.
    /// </summary>
    public void WriteAllBytes(byte[] bytes) => File.WriteAllBytes(Value, bytes);

    /// <summary>
    ///     Appends the given text to the file, creating it if missing.
    /// </summary>
    public void AppendAllText(string contents) => File.AppendAllText(Value, contents);

    /// <summary>
    ///     Deletes the file.
    /// </summary>
    public void Delete() => File.Delete(Value);

    /// <summary>
    ///     Copies the file to a destination path.
    /// </summary>
    /// <param name="dest">The destination path.</param>
    /// <param name="overwrite">
    ///     <see langword="true" /> to overwrite an existing destination; otherwise <see langword="false" />.
    /// </param>
    public void CopyTo(string dest, bool overwrite = false) => File.Copy(Value, dest, overwrite);

    /// <summary>
    ///     Moves the file to a destination path.
    /// </summary>
    /// <param name="dest">The destination path.</param>
    /// <param name="overwrite">
    ///     <see langword="true" /> to overwrite an existing destination; otherwise <see langword="false" />.
    /// </param>
    public void MoveTo(string dest, bool overwrite = false) {
#if NETSTANDARD2_1
        if (overwrite && File.Exists(dest))
            File.Delete(dest);
        File.Move(Value, dest);
#else
        File.Move(Value, dest, overwrite);
#endif
    }

    /// <summary>
    ///     Opens the file for reading.
    /// </summary>
    public Stream OpenRead() => File.OpenRead(Value);

    /// <summary>
    ///     Opens the file for writing, creating it if missing.
    /// </summary>
    public Stream OpenWrite() => File.OpenWrite(Value);

    /// <summary>
    ///     Converts this <see cref="FilePath" /> to its underlying path string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    ///     Converts a <see cref="FilePath" /> implicitly to its underlying path string.
    /// </summary>
    public static implicit operator string(FilePath path) => path.Value;
}
