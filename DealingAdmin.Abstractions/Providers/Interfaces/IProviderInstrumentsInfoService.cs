using DealingAdmin.Abstractions.Models.ProviderInstruments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IProviderInstrumentsInfoService
{
    Task<Dictionary<string, string>> Instruments(string providerId);
    Task<IEnumerable<ProviderInstrumentInfo>> InstrumentsInfo(string providerId);
}