namespace SchoolAccount.E2ETests.Common.Fixtures;

[CollectionDefinition(Name)]
public class PublicSiteCollection : ICollectionFixture<BrowserFixture>
{
    public const string Name = "Public Site Collection";
}

[CollectionDefinition(Name)]
public class SchoolAccountCollection : ICollectionFixture<SchoolAccountBrowserFixture>
{
    public const string Name = "School Account Collection";
}
