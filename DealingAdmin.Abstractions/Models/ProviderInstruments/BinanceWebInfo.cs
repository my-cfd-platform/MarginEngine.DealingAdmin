using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Models.ProviderInstruments;

public class BinanceWebInfo
{
    public Pagedata pageData { get; set; }
}

public class Pagedata
{
    public Redux redux { get; set; }
}

public class Redux
{
    public Products products { get; set; }
}

public class Products
{
    public bool loading { get; set; }
    public Dictionary<string, BinanceProduct> productMap { get; set; }
    //public object[] hotProduct { get; set; }
    public long updateTime { get; set; }
}

public class BinanceProduct
{
    public string low { get; set; }
    public string close { get; set; }
    public string high { get; set; }
    public string open { get; set; }
    public string volume { get; set; }
    public string quoteVolume { get; set; }
    public string symbol { get; set; }
    public string tickSize { get; set; }
    public string minQty { get; set; }
    public string quoteAsset { get; set; }
    public string baseAsset { get; set; }
    public string quoteName { get; set; }
    public string baseAssetName { get; set; }
    public string parentMarket { get; set; }
    public string parentMarketName { get; set; }
    public long circulatingSupply { get; set; }
    public string[] tags { get; set; }
}