using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IDefaultFavoriteInstruments
{
    public string Id { get; }
    public IEnumerable<string> Instruments { get; }
}