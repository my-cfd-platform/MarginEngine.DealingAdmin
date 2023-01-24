using System.Collections.Generic;
using DealingAdmin.Abstractions.Models.LpSettings;
using DealingAdmin.Shared.Services.Providers.Interfaces;
using MyNoSqlServer.Abstractions;

namespace DealingAdmin.Shared.Services.Providers.Models;

public class ProviderInstrumentMapMyNoSqlEntity : MyNoSqlDbEntity, IProviderInstrumentMap
{
    public string Id => RowKey;

    public string LpId { get; set; }
    public IDictionary<string, string> Map { get; set; }


    public static string GeneratePartitionKey()
    {
        return "im";
    }

    public static string GenerateRowKey(string id)
    {
        return id;
    }
    public static ProviderInstrumentMapMyNoSqlEntity Create(IProviderInstrumentMap src)
    {
        return new ProviderInstrumentMapMyNoSqlEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(src.LpId),
            LpId = src.LpId,
            Map = src.Map
        };
    }
}