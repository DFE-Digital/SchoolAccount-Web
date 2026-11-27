namespace SchoolAccount.E2ETests.Common.Database;

public sealed class CollectReturnStatusTable(LocalDatabase database)
{
    private const string TableName = "CollectReturnStatus";

    public async Task<int> AddAsync(CollectReturnStatus row)
    {
        return (int)await database.InsertAndReturnIdAsync(row, TableName);
    }

    public Task RemoveAsync(params int[] ids)
    {
        return database.DeleteByIdsAsync(TableName, ids);
    }
}
