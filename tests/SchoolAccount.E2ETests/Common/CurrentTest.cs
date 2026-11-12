namespace SchoolAccount.E2ETests.Common;

public static class CurrentTest
{
    public static bool Failed => TestContext.Current.TestState?.Result == TestResult.Failed;

    public static string ClassName =>
        TestContext.Current.TestClass?.TestClassSimpleName ?? "UnknownClass";

    public static string MethodName =>
        TestContext.Current.Test?.TestCase?.TestMethod?.MethodName ?? "UnknownMethod";
}
