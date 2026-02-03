// This MSTest shim is only compiled when the 'USE_TEST_SHIMS' symbol is defined.
// Define the symbol in a build environment that intentionally lacks MSTest assemblies.
#if USE_TEST_SHIMS
using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestClassAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestMethodAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestInitializeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestCleanupAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ExpectedExceptionAttribute : Attribute
    {
        public Type ExceptionType { get; }
        public ExpectedExceptionAttribute(Type exceptionType) { ExceptionType = exceptionType; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCategoryAttribute : Attribute
    {
        public string Category { get; }
        public TestCategoryAttribute(string category) { Category = category; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DataRowAttribute : Attribute
    {
        public object[] Data { get; }
        public DataRowAttribute(params object[] data) { Data = data; }
    }
}
#endif
