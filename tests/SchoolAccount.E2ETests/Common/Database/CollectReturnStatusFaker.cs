using Bogus;

namespace SchoolAccount.E2ETests.Common.Database;

public static class CollectReturnStatusFaker
{
    private static readonly Faker<CollectReturnStatus> Faker =
        new Faker<CollectReturnStatus>().CustomInstantiator(f => new CollectReturnStatus
        {
            SchoolName = $"[TEST] {f.Company.CompanyName()} School",
            LAEStab = $"999/{f.Random.Number(1000, 9999)}",
            ReturnStatusCode = f.Random.Number(1, 5),
            Errors = f.Random.Number(0, 10),
            Queries = f.Random.Number(0, 10),
            OkdErrorsQueries = f.Random.Number(0, 10),
            Hash = Guid.NewGuid().ToString(),
            DCID = f.Random.Number(1, 999999),
            Collection = f.PickRandom("Census", "SEN2", "Workforce", "AlternativeProvision"),
            DataReturnID = f.Random.Number(1, 999999),
        });

    public static CollectReturnStatus Generate() => Faker.Generate();
}
