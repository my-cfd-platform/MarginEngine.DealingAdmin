using System.Collections.Generic;
using System.Threading.Tasks;
using DealingAdmin.Shared.Services.Providers.Interfaces;
using DealingAdmin.Shared.Services.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;

namespace DealingAdmin.Shared.Services.Providers;

public class InstrumentMappingRepository : IInstrumentMappingRepository
{
    private readonly IMyNoSqlServerDataWriter<ProviderInstrumentMapMyNoSqlEntity> _table;
    private const string TableName = "instrument-mapping";
    public InstrumentMappingRepository(string url)
    {
        var table = new MyNoSqlServerDataWriter<ProviderInstrumentMapMyNoSqlEntity>(
            () => url,
            TableName,
            true);
        // create table if not exists
        table.CreateTableIfNotExistsAsync().GetAwaiter().GetResult();
        _table = table;
    }

    public async Task<IEnumerable<IProviderInstrumentMap>> GetAllAsync()
    {
        var partitionKey = ProviderInstrumentMapMyNoSqlEntity.GeneratePartitionKey();
        return await _table.GetAsync(partitionKey);
    }

    public async Task UpdateAsync(IProviderInstrumentMap item)
    {
        var entity = ProviderInstrumentMapMyNoSqlEntity.Create(item);
        await _table.InsertOrReplaceAsync(entity);
    }
}