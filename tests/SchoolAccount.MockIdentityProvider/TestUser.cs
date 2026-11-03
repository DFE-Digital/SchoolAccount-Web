using System.Text.Json;

namespace SchoolAccount.MockIdentityProvider;

internal sealed record TestUser
{
    public required string Id { get; init; }

    public required Guid Sub { get; init; }

    public required string GivenName { get; init; }

    public required string FamilyName { get; init; }

    public required string Email { get; init; }

    /// <summary>
    /// Sent as-is in the <c>organisation</c> claim, in the shape DfE Sign-in uses.
    /// </summary>
    public required JsonElement Organisation { get; init; }

    public string OrganisationName => Organisation.GetProperty("name").GetString() ?? Id;
}

internal sealed class TestUserStore(IReadOnlyList<TestUser> users)
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public IReadOnlyList<TestUser> Users { get; } = users;

    public TestUser? Find(string id) =>
        Users.FirstOrDefault(user => string.Equals(user.Id, id, StringComparison.Ordinal));

    public static TestUserStore Load(string path)
    {
        using var stream = File.OpenRead(path);
        var users =
            JsonSerializer.Deserialize<List<TestUser>>(stream, _options)
            ?? throw new InvalidOperationException($"No users found in {path}.");

        return new TestUserStore(users);
    }
}
