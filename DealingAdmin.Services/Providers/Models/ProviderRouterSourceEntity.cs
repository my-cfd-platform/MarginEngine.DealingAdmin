using DealingAdmin.Abstractions.Providers.Interfaces;
using MyNoSqlServer.Abstractions;

namespace DealingAdmin.Services.Providers.Models;

public class ProviderRouterSourceEntity : MyNoSqlDbEntity, IProviderRouterSource
{
    public string Id => RowKey;

    public string LpId { get; set; }
    public string RemoteUrl { get; set; }
    public List<string> InstrumentIds {get;set;}

    public static string GeneratePartitionKey()
    {
        return "prs";
    }

    public static string GenerateRowKey(string id)
    {
        return id;
    }

    public static ProviderRouterSourceEntity Create(IProviderRouterSource src)
    {
        return new ProviderRouterSourceEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(src.LpId),
            LpId = src.LpId,
            RemoteUrl = src.RemoteUrl,
            InstrumentIds = src.InstrumentIds
        };
    }
}