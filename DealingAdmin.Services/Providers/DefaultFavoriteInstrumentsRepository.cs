using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;

namespace DealingAdmin.Services.Providers;

public class DefaultFavoriteInstrumentsRepository: IRepository<IDefaultFavoriteInstruments>
{
    private readonly IMyNoSqlServerDataWriter<DefaultFavoriteInstrumentsEntity> _table;
    private const string TableName = "defaultvalues";
    public DefaultFavoriteInstrumentsRepository(string url)
    {
        var table = new MyNoSqlServerDataWriter<DefaultFavoriteInstrumentsEntity>(
            () => url,
            TableName,
            true);
        // create table if not exists
        table.CreateTableIfNotExistsAsync().GetAwaiter().GetResult();
        _table = table;
    }

    public async Task<IEnumerable<IDefaultFavoriteInstruments>> GetAllAsync()
    {
        var partitionKey = DefaultFavoriteInstrumentsEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey);
    }

    public async Task UpdateAsync(IDefaultFavoriteInstruments item)
    {
        var entity = DefaultFavoriteInstrumentsEntity.Create(item);
        await _table.InsertOrReplaceAsync(entity);
    }

    public async Task<IDefaultFavoriteInstruments> GetAsync(string key)
    {
        var partitionKey = DefaultFavoriteInstrumentsEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey,key);
    }
}