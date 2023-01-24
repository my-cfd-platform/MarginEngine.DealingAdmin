using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealingAdmin.Shared.Services.Providers.Interfaces;

public interface IInstrumentMappingRepository
{
    Task<IEnumerable<IProviderInstrumentMap>> GetAllAsync();
    Task UpdateAsync(IProviderInstrumentMap item);
}