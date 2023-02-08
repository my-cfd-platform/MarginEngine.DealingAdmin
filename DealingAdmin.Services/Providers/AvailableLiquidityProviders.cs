using DealingAdmin.Abstractions.Extensions;
using DealingAdmin.Abstractions.Providers.Interfaces;

namespace DealingAdmin.Services.Providers;

    public class AvailableLiquidityProviders : IAvailableLiquidityProviders
    {
        private readonly IEnumerable<string> _providers;

        public AvailableLiquidityProviders(string settingString)
        {
            if (settingString.IsNullOrEmpty())
                return;
            _providers = settingString.Split(",");
        }

        public IEnumerable<string> GetLiquidityProviders()
        {
            return _providers;
        }
    }
