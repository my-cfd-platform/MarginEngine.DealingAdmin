using System.Collections.Generic;
using System.Threading.Tasks;
using DealingAdmin.Abstractions.Models.LpSettings;

namespace DealingAdmin.Shared.Services.Providers.Interfaces
{
    public interface IInstrumentMappingRepository
    {
        Task<IEnumerable<ProviderInstrumentMap>> GetAllAsync();

        Task<bool> Set(string provider, string instrument, string mapName);

        Task<bool> Delete();
    }
}