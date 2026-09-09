using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace ValidSphere;

/// <summary>
///     Provides file-system path assertions.
/// </summary>
/// <remarks>
///     Strings switch into file or directory mode through <see cref="AsFile{TPolicy}(Assertion{string,TPolicy},string?)" />
///     and <see cref="AsDirectory{TPolicy}(Assertion{string,TPolicy},string?)" />, which extract the raw
///     <see cref="FilePath" />/<see cref="DirectoryPath" /> carriers; chainable path assertions start at
///     <c>path.Is().File()</c> and <c>path.Is().Directory()</c>. All path checks run on those
///     mode types so file semantics are never applied to a directory and vice versa.
/// </remarks>
public static class PathAssertions {
    /// <summary>
    ///     Validates the asserted path is not <see langword="null" /> and extracts the raw <see cref="FilePath" /> carrier.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message for the null branch. When <see langword="null" />, the default message is used.
    /// </param>
    /// <returns>The raw <see cref="FilePath" />.</returns>
    /// <remarks>
    ///     Terminal extractor without further validation: for chainable file assertions use <c>path.Is().File()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FilePath AsFile<TPolicy>(this Assertion<string, TPolicy> assertion, string? message = null)
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Path must not be null.");

        return new FilePath(value);
    }

    /// <summary>
    ///     Validates the asserted path is not <see langword="null" /> and extracts the raw <see cref="DirectoryPath" /> carrier.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message for the null branch. When <see langword="null" />, the default message is used.
    /// </param>
    /// <returns>The raw <see cref="DirectoryPath" />.</returns>
    /// <remarks>
    ///     Terminal extractor without further validation: for chainable directory assertions use <c>path.Is().Directory()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DirectoryPath AsDirectory<TPolicy>(this Assertion<string, TPolicy> assertion, string? message = null)
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Path must not be null.");

        return new DirectoryPath(value);
    }

    /// <summary>
    ///     Switches the asserted path into file mode and returns a chainable file assertion.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message for the null branch. When <see langword="null" />, the default message is used.
    /// </param>
    /// <returns>An assertion over the <see cref="FilePath" />.</returns>
    /// <remarks>
    ///     Chain entry without further validation: for the raw carrier use <c>AsFile()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> File<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Path must not be null.");

        return assertion.Refine(new FilePath(value));
    }

    /// <summary>
    ///     Switches the asserted path into directory mode and returns a chainable directory assertion.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message for the null branch. When <see langword="null" />, the default message is used.
    /// </param>
    /// <returns>An assertion over the <see cref="DirectoryPath" />.</returns>
    /// <remarks>
    ///     Chain entry without further validation: for the raw carrier use <c>AsDirectory()</c> instead.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> Directory<TPolicy>(
        this Assertion<string, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value;

        if (value is null)
            assertion.FailNull(message ?? "Path must not be null.");

        return assertion.Refine(new DirectoryPath(value));
    }

    /// <summary>
    ///     Asserts that the file exists.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> Exists<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!System.IO.File.Exists(assertion.Value.Value))
            assertion.Fail(message ?? "File must exist.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory exists.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> Exists<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!System.IO.Directory.Exists(assertion.Value.Value))
            assertion.Fail(message ?? "Directory must exist.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file name has the given extension.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="extension">
    ///     The expected extension, with or without a leading dot, compared case-insensitively by default.
    /// </param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> HaveExtension<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string extension,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        return assertion.HaveExtension([extension], comparison, message);
    }

    /// <summary>
    ///     Asserts that the file name has one of the given extensions.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="extensions">
    ///     The allowed extensions, each with or without a leading dot, compared case-insensitively by default.
    /// </param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> HaveExtension<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string[] extensions,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(extensions, nameof(extensions));

        if (extensions.Length == 0)
            throw new ArgumentException("At least one extension is required.", nameof(extensions));

        var actual = Path.GetExtension(assertion.Value.Value);

        for (var i = 0; i < extensions.Length; i++) {
            var expected = "." + (extensions[i] ?? string.Empty).TrimStart('.');

            if (actual.Equals(expected, comparison))
                return assertion;
        }

        assertion.Fail(message ?? $"Path must have extension '{string.Join(", ", extensions)}'.");
        return assertion;
    }

    /// <summary>
    ///     Asserts that the file name equals the given name.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="fileName">The expected file name, including its extension.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> HaveFileName<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string fileName,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(fileName, nameof(fileName));

        if (!Path.GetFileName(assertion.Value.Value).Equals(fileName, comparison))
            assertion.Fail(message ?? $"Path must have file name '{fileName}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file path is absolute (fully qualified).
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> Absolute<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!Path.IsPathFullyQualified(assertion.Value.Value))
            assertion.Fail(message ?? "Path must be absolute.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory path is absolute (fully qualified).
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> Absolute<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (!Path.IsPathFullyQualified(assertion.Value.Value))
            assertion.Fail(message ?? "Path must be absolute.");

        return assertion;
    }
    /// <summary>
    ///     Asserts that the file does not exist.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> NotExists<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (System.IO.File.Exists(assertion.Value.Value))
            assertion.Fail(message ?? "File must not exist.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory does not exist.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> NotExists<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (System.IO.Directory.Exists(assertion.Value.Value))
            assertion.Fail(message ?? "Directory must not exist.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file name has no extension.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> NoExtension<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (Path.GetExtension(assertion.Value.Value).Length != 0)
            assertion.Fail(message ?? "Path must have no extension.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory name equals the given name.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="name">The expected directory name, without any parent path.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     A trailing directory separator is ignored, so <c>a/b/</c> has the name <c>b</c>.
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> HaveName<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string name,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(name, nameof(name));

        if (!Path.GetFileName(TrimTrailingSeparator(assertion.Value.Value)).Equals(name, comparison))
            assertion.Fail(message ?? $"Directory must have name '{name}'.");

        return assertion;
    }
    /// <summary>
    ///     Asserts that the file is empty (zero bytes long).
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> Empty<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        if (new FileInfo(value).Length != 0)
            assertion.Fail(message ?? "File must be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file is not empty.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> NotEmpty<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        if (new FileInfo(value).Length == 0)
            assertion.Fail(message ?? "File must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file has exactly the specified length in bytes.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expected">The required file length in bytes.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> Length<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        long expected,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        var actual = new FileInfo(value).Length;

        if (actual != expected) {
            assertion.Fail(message ?? $"Expected length '{expected}', but found '{actual}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file is at least the specified length in bytes.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted file length in bytes.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> MinLength<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        long minimum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        if (new FileInfo(value).Length < minimum) {
            assertion.Fail(message ?? $"File length must be at least '{minimum}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file is at most the specified length in bytes.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="maximum">The maximum permitted file length in bytes.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> MaxLength<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        long maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        if (new FileInfo(value).Length > maximum) {
            assertion.Fail(message ?? $"File length must be at most '{maximum}'.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file length in bytes lies within the inclusive range
    ///     <paramref name="minimum" /> through <paramref name="maximum" />.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="minimum">The minimum permitted file length in bytes.</param>
    /// <param name="maximum">The maximum permitted file length in bytes.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> LengthInRange<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        long minimum,
        long maximum,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        var value = assertion.Value.Value;

        if (!System.IO.File.Exists(value))
            assertion.Fail(message ?? "File must exist.");

        var actual = new FileInfo(value).Length;

        if (actual < minimum || actual > maximum) {
            assertion.Fail(message ?? $"File length must be in range [{minimum}, {maximum}].");
        }

        return assertion;
    }
    /// <summary>
    ///     Asserts that the directory is empty (contains no files or subdirectories).
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> Empty<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (System.IO.Directory.GetFileSystemEntries(assertion.Value.Value).Length != 0)
            assertion.Fail(message ?? "Directory must be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory is not empty.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> NotEmpty<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        if (System.IO.Directory.GetFileSystemEntries(assertion.Value.Value).Length == 0)
            assertion.Fail(message ?? "Directory must not be empty.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory contains a file with the given name.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="fileName">The required file name, relative to the directory.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     Name matching follows operating-system rules (case-insensitive on Windows, case-sensitive on Linux).
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> ContainsFile<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string fileName,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(fileName, nameof(fileName));

        if (!System.IO.File.Exists(Path.Combine(assertion.Value.Value, fileName)))
            assertion.Fail(message ?? $"Directory must contain file '{fileName}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the directory contains a subdirectory with the given name.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="directoryName">The required subdirectory name, relative to the directory.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    /// <remarks>
    ///     Name matching follows operating-system rules (case-insensitive on Windows, case-sensitive on Linux).
    /// </remarks>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> ContainsDirectory<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string directoryName,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(directoryName, nameof(directoryName));

        if (!System.IO.Directory.Exists(Path.Combine(assertion.Value.Value, directoryName)))
            assertion.Fail(message ?? $"Directory must contain directory '{directoryName}'.");

        return assertion;
    }
    /// <summary>
    ///     Asserts that the normalized absolute file path equals the given path.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expectedFullPath">The expected path; both sides are normalized with <see cref="Path.GetFullPath(string)" />.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> HaveFullPath<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string expectedFullPath,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(expectedFullPath, nameof(expectedFullPath));

        if (!Path.GetFullPath(assertion.Value.Value).Equals(Path.GetFullPath(expectedFullPath), comparison))
            assertion.Fail(message ?? $"Path must have full path '{expectedFullPath}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the normalized absolute directory path equals the given path.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="expectedFullPath">The expected path; both sides are normalized with <see cref="Path.GetFullPath(string)" />.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<DirectoryPath, TPolicy> HaveFullPath<TPolicy>(
        this Assertion<DirectoryPath, TPolicy> assertion,
        string expectedFullPath,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(expectedFullPath, nameof(expectedFullPath));

        if (!Path.GetFullPath(assertion.Value.Value).Equals(Path.GetFullPath(expectedFullPath), comparison))
            assertion.Fail(message ?? $"Path must have full path '{expectedFullPath}'.");

        return assertion;
    }

    /// <summary>
    ///     Asserts that the file is located directly inside the given directory.
    /// </summary>
    /// <typeparam name="TPolicy">The assertion failure policy.</typeparam>
    /// <param name="assertion">The current assertion.</param>
    /// <param name="directory">The expected parent directory; both sides are normalized with <see cref="Path.GetFullPath(string)" />.</param>
    /// <param name="comparison">The string comparison to use. Defaults to <see cref="StringComparison.OrdinalIgnoreCase" />.</param>
    /// <param name="message">
    ///     An optional custom failure message. When <see langword="null" />, the default assertion message is used.
    /// </param>
    /// <returns>The original assertion for further chaining or implicit value extraction.</returns>
    [DebuggerStepThrough]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Assertion<FilePath, TPolicy> InDirectory<TPolicy>(
        this Assertion<FilePath, TPolicy> assertion,
        string directory,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        string? message = null
    )
        where TPolicy : struct, IAssertionPolicy {
        ThrowHelper.ThrowIfNull(directory, nameof(directory));

        var parent = Path.GetDirectoryName(Path.GetFullPath(assertion.Value.Value));

        if (parent is null || !parent.Equals(TrimTrailingSeparator(Path.GetFullPath(directory)), comparison))
            assertion.Fail(message ?? $"File must be in directory '{directory}'.");

        return assertion;
    }

    /// <summary>
    ///     Removes a single trailing directory separator, if present. Never touches roots.
    /// </summary>
    private static string TrimTrailingSeparator(string path) {
        if (path.Length > 1
            && (path[path.Length - 1] == Path.DirectorySeparatorChar
                || path[path.Length - 1] == Path.AltDirectorySeparatorChar))
            return path.Substring(0, path.Length - 1);
        return path;
    }
}
