using System.Runtime.Serialization;
using SimpleTrading.Abstraction.Candles;

namespace DealingAdmin.Services.Contracts;

[DataContract]
public class UpdateCandlesHistoryServiceBusContract
{
    [DataMember(Order = 1)]
    public string InstrumentId { get; set; }
    [DataMember(Order = 2)]
    public CandleType CandleType { get; set; }
    [DataMember(Order = 3)]
    public DateTime DateFrom { get; set; }
    [DataMember(Order = 4)]
    public DateTime DateTo { get; set; }
    [DataMember(Order = 5)]
    public bool CacheIsUpdated { get; set; }
}