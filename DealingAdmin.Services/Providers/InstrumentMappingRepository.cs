using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;

namespace DealingAdmin.Services.Providers;

public class InstrumentMappingRepository : IRepository<IProviderInstrumentMap>
{
    private readonly IMyNoSqlServerDataWriter<ProviderInstrumentEntity> _table;
    private const string TableName = "instrument-mapping";
    public InstrumentMappingRepository(string url)
    {
        var table = new MyNoSqlServerDataWriter<ProviderInstrumentEntity>(
            () => url,
            TableName,
            true);
        // create table if not exists
        table.CreateTableIfNotExistsAsync().GetAwaiter().GetResult();
        _table = table;
    }

    public async Task<IEnumerable<IProviderInstrumentMap>> GetAllAsync()
    {
        var partitionKey = ProviderInstrumentEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey);
    }

    public async Task UpdateAsync(IProviderInstrumentMap item)
    {
        var entity = ProviderInstrumentEntity.Create(item);
        await _table.InsertOrReplaceAsync(entity);
    }
}