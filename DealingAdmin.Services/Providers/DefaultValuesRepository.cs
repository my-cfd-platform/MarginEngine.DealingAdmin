using System.Dynamic;
using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;

namespace DealingAdmin.Services.Providers;

public class DefaultValuesRepository: IRepository<IDefaultValues>
{
    private readonly IMyNoSqlServerDataWriter<DefaultValuesEntity> _table;
    private const string TableName = "defaultvalues";
    public DefaultValuesRepository(string url)
    {
        var table = new MyNoSqlServerDataWriter<DefaultValuesEntity>(
            () => url,
            TableName,
            true);
        // create table if not exists
        table.CreateTableIfNotExistsAsync().GetAwaiter().GetResult();
        _table = table;
    }

    public async Task<IEnumerable<IDefaultValues>> GetAllAsync()
    {
        var partitionKey = DefaultValuesEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey);
    }

    public async Task UpdateAsync(IDefaultValues item)
    {
        var entity = DefaultValuesEntity.Create(item);
        await _table.InsertOrReplaceAsync(entity);
    }

    public async Task<IDefaultValues> GetAsync(string key)
    {
        var partitionKey = DefaultValuesEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey,key);
    }
}