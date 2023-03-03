using System.Collections.Generic;
using DealingAdmin.Abstractions.Providers.Interfaces;
using MyNoSqlServer.Abstractions;

namespace DealingAdmin.Abstractions.Providers.Models;

public class DefaultFavoriteInstrumentsEntity : MyNoSqlDbEntity, IDefaultFavoriteInstruments
{
    public string Id
    {
        get => RowKey;
        set => RowKey = value;
    }

    public IEnumerable<string> Instruments { get; set; }

    public static string GeneratePartitionKey()
    {
        return "fi";
    }

    public static DefaultFavoriteInstrumentsEntity Create(IDefaultFavoriteInstruments src)
    {
        return new DefaultFavoriteInstrumentsEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = src.Id,
            Instruments = src.Instruments,
        };
    }

    public static DefaultFavoriteInstrumentsEntity CreateMobile(IEnumerable<string> instruments)
    {
        return new DefaultFavoriteInstrumentsEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = "mobile",
            Instruments = instruments,
        };
    }

    public static DefaultFavoriteInstrumentsEntity CreateWeb(IEnumerable<string> instruments)
    {
        return new DefaultFavoriteInstrumentsEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = "web",
            Instruments = instruments,
        };
    }
}