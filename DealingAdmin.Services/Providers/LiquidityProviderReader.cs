using DealingAdmin.Abstractions;
using DealingAdmin.Abstractions.Extensions;
using Flurl;
using Flurl.Http;

namespace DealingAdmin.Services.Providers;
public class LiquidityProviderReader : ILiquidityProviderReader
{
    private readonly string _feedRouterUrl;

    public LiquidityProviderReader(string quoteFeedRouterUrl)
    {
        _feedRouterUrl = quoteFeedRouterUrl;
    }

    public async Task<IEnumerable<string>> GetLiquidityProviders()
    {
        return _feedRouterUrl.IsNotNullOrEmpty()
            ? await _feedRouterUrl
                .AppendPathSegments("api", "LiquidityProviders", "All")
                .GetJsonAsync<List<string>>()
            : new List<string>();
    }
}
