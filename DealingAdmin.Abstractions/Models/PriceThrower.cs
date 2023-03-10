
using System;
using DealingAdmin.Abstractions.Interfaces;
using SimpleTrading.Abstraction.BidAsk;

namespace DealingAdmin.Abstractions.Models
{
    public class PriceThrower : IBidAskSb
    {
        public string Id { get; set; }
        
        //DateTime IBidAskSbModel.DateTime => DateTime.UtcNow;
        ulong IBidAskSb.JsUnixTime =>
            (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        
        public double Bid { get; set; }
        
        public double Ask { get; set; }
        
        public string ApiKey { get; set; }
    }
}