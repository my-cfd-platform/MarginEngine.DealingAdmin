using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealingAdmin.Abstractions.Models.LpSettings;
using DealingAdmin.Shared.Services.Providers.Interfaces;

namespace DealingAdmin.Shared.Services.Providers
{
    public class InstrumentMappingRepository : IInstrumentMappingRepository
    {
        private readonly string _feedRouterUrl;

        public InstrumentMappingRepository(string routerUrl)
        {
            _feedRouterUrl = routerUrl;
        }

        public async Task<IEnumerable<ProviderInstrumentMap>> GetAllAsync()
        {
            //todo load from storage
            return new List<ProviderInstrumentMap>
            {
                new()
                {
                    LpId = "Binance",
                    Map = new()
                    {
                        { "AAVEUSD","AAVEUSD.B"  }
                    }
                },
                new()
                {
                    LpId = "Polygon",
                    Map = new()
                    {
                        { "AMZN","AMAZON" },
                        { "ALGOUSD","ALGOUSD.P" }
                    }
                }
            };
            return await _feedRouterUrl
                .AppendPathSegments("api", "ProviderInstrumentMapping", "All")
                .GetJsonAsync<List<ProviderInstrumentMap>>();
        }
        public async Task<bool> Set(string provider, string instrument, string mapName)
        {
            //todo save item
            return true;
        }

        public async Task<bool> Delete()
        {
            //todo delete item
            return true;
        }
    }
}