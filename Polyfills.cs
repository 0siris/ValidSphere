using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValidSphere
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when a value argument is unexpectedly <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// Behavior-identical to <see cref="ArgumentNullException.ThrowIfNull(object?, string?)"/>.
    /// A single shared code path keeps both target frameworks free of conditional code at the call sites.
    /// </remarks>
    internal static class ThrowHelper
    {
        [DebuggerStepThrough]
        [StackTraceHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull(object? value, string? paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName);
        }
    }
}
