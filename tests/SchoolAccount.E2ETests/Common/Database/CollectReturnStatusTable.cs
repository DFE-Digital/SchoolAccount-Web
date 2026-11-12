namespace SchoolAccount.E2ETests.Common.Database;

public sealed class CollectReturnStatusTable(TestDatabase database)
{
    private const string TableName = "CollectReturnStatus";

    public async Task<int> AddAsync(CollectReturnStatus row) =>
        (int)await database.InsertAndReturnIdAsync(row, TableName);

    public Task RemoveAsync(params int[] ids) => database.DeleteByIdsAsync(TableName, ids);
}
