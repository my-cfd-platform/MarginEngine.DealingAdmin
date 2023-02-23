using System;

namespace DealingAdmin.Abstractions.Interfaces;

public interface IBidAskSb
{
    public string Id { get; }
    public ulong JsUnixTime { get; }
    double Bid { get; set; }
    double Ask { get; set; }
}