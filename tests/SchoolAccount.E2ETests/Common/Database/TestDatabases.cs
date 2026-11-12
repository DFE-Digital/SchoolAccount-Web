using SchoolAccount.E2ETests.Common.Configuration;

namespace SchoolAccount.E2ETests.Common.Database;

public static class TestDatabases
{
    public static TestDatabase CollectPortal(DatabaseSettings settings) =>
        new(settings, "COLLECTPortal");

    public static TestDatabase CollectStateLedger(DatabaseSettings settings) =>
        new(settings, "CollectStateLedger");
}
