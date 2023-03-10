using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IAvailableLiquidityProviders
{
    IEnumerable<string> GetLiquidityProviders();
}