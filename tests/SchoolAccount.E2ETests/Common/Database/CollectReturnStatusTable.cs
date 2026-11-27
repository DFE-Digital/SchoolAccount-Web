namespace SchoolAccount.E2ETests.Common.Database;

public sealed class CollectReturnStatusTable(LocalDatabase database)
{
    private const string _tableName = "CollectReturnStatus";

    public async Task<int> AddAsync(CollectReturnStatus row)
    {
        return (int)await database.InsertAndReturnIdAsync(row, _tableName);
    }

    public Task RemoveAsync(params int[] ids)
    {
        return database.DeleteByIdsAsync(_tableName, ids);
    }
}
