namespace SchoolAccount.E2ETests.Common.Artifacts;

/// <summary>Details of the test that is running now, read from xUnit's test context.</summary>
public static class CurrentTest
{
    /// <summary>
    /// Whether the test failed. Only known once the test has finished, so read it while the test
    /// class is being disposed.
    /// </summary>
    public static bool Failed => TestContext.Current.TestState?.Result is TestResult.Failed;

    public static string ClassName =>
        TestContext.Current.TestClass?.TestClassSimpleName ?? "UnknownClass";

    public static string MethodName => TestContext.Current.TestMethod?.MethodName ?? "UnknownTest";
}
