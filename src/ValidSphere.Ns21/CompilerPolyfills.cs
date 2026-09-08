using System;

namespace System.Runtime.CompilerServices {
    /// <summary>
    /// Polyfill for the compiler-recognized caller-expression attribute (.NET 6+), missing on netstandard2.1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute {
        public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;
        public string ParameterName { get; }
    }
}

namespace System.Diagnostics {
    /// <summary>
    /// Polyfill for the stack-trace-hiding attribute (.NET 5+), missing on netstandard2.1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class StackTraceHiddenAttribute : Attribute {
    }
}
