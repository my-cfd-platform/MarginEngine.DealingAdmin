using DealingAdmin.Abstractions.Extensions;
using DealingAdmin.Abstractions.Interfaces;
using SimpleTrading.Abstraction.BidAsk;
using System.Runtime.Serialization;

namespace DealingAdmin.Abstractions.Models.BidAskSb
{
    [DataContract]
    public class UnfilteredBidAskSb : IBidAskSb
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }
        [DataMember(Order = 2)]
        public ulong JsUnixTime { get; set; }
        [DataMember(Order = 3)]
        public double Bid { get; set; }
        [DataMember(Order = 4)]
        public double Ask { get; set; }
        [DataMember(Order = 5)]
        public string LiquidityProvider { get; set; }
        public static BidAskSb Create(IBidAskSb src)
        {
            if (src == null)
                return default;
            return new()
            {
                Id = src.Id,
                Ask = src.Ask,
                Bid = src.Bid,
                JsUnixTime = src.JsUnixTime
            };
        }

        public IUnfilteredBidAsk ToUnfilteredIBidAsk()
        {
            return new UnfilteredBidAsk
            {
                Id = Id,
                DateTime = JsUnixTime.JsUnixTimeToDateTime(),
                Bid = Bid,
                Ask = Ask,
                LiquidityProvider = LiquidityProvider
            };
        }
    }
}
