using System;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestFixtureAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class SetUpAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TearDownAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCaseAttribute : Attribute
    {
        public object[] Arguments { get; }
        public TestCaseAttribute(params object[] args) { Arguments = args; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCaseSourceAttribute : Attribute
    {
        public string SourceName { get; }
        public TestCaseSourceAttribute(string sourceName) { SourceName = sourceName; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestCaseDataAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class IgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class CategoryAttribute : Attribute
    {
        public string Name { get; }
        public CategoryAttribute(string name) { Name = name; }
    }

    public static class Assert
    {
        public static void IsNotNull(object obj) { if (obj == null) throw new Exception("Assert.IsNotNull failed"); }
        public static void IsTrue(bool cond) { if (!cond) throw new Exception("Assert.IsTrue failed"); }
        public static void IsFalse(bool cond) { if (cond) throw new Exception("Assert.IsFalse failed"); }
        public static void AreEqual(object expected, object actual) { if (!object.Equals(expected, actual)) throw new Exception($"Assert.AreEqual failed. Expected: {expected}, Actual: {actual}"); }
        public static void Throws<T>(Action action) where T : Exception
        {
            try { action(); }
            catch (Exception ex) { if (ex is T) return; throw; }
            throw new Exception($"Assert.Throws failed. Exception of type {typeof(T).Name} was not thrown.");
        }
    }
}
