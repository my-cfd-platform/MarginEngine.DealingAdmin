using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;

namespace DealingAdmin.Services.Providers;

public class ProviderRouterSourceRepository : IRepository<IProviderRouterSource>
{
    private readonly IMyNoSqlServerDataWriter<ProviderRouterSourceEntity> _table;
    private const string TableName = "provider-router-source";
    public ProviderRouterSourceRepository(string url)
    {
        var table = new MyNoSqlServerDataWriter<ProviderRouterSourceEntity>(
            () => url,
            TableName,
            true);
        // create table if not exists
        table.CreateTableIfNotExistsAsync().GetAwaiter().GetResult();
        _table = table;
    }

    public async Task<IEnumerable<IProviderRouterSource>> GetAllAsync()
    {
        var partitionKey = ProviderRouterSourceEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey);
    }

    public async Task<IProviderRouterSource> GetAsync(string key)
    {
        var partitionKey = ProviderRouterSourceEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey, key);
    }

    public async Task UpdateAsync(IProviderRouterSource item)
    {
        var entity = ProviderRouterSourceEntity.Create(item);
        await _table.InsertOrReplaceAsync(entity);
    }
}