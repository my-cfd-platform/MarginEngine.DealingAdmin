using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Models.LpSettings;

public class ProviderInstrumentMap
{
    public string LpId { get; set; }
    public Dictionary<string, string> Map { get; set; }
}