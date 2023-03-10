using System.Collections.Generic;
using DealingAdmin.Abstractions.Providers.Interfaces;
using MyNoSqlServer.Abstractions;

namespace DealingAdmin.Abstractions.Providers.Models;

public class DefaultValuesEntity : MyNoSqlDbEntity, IDefaultValues
{
    public string Id
    {
        get => RowKey;
        set => RowKey = value;
    }

    public string Value { get; set; } = string.Empty;
    public IEnumerable<string> Values { get; set; }

    public static string GeneratePartitionKey()
    {
        return "dv";
    }

    public static DefaultValuesEntity Create(IDefaultValues src)
    {
        return new DefaultValuesEntity
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = src.Id,
            Value = src.Value,
            Values = src.Values
        };
    }
}