﻿namespace DealingAdmin.Abstractions.Models.ProviderInstruments;

public class BinanceInfo
{
    public BinanceSymbol[] symbols { get; set; }
}

public class BinanceSymbol
{
    public string symbol { get; set; }
    public string status { get; set; }
    public string baseAsset { get; set; }
    public int baseAssetPrecision { get; set; }
    public string quoteAsset { get; set; }
    public int quotePrecision { get; set; }
    public int quoteAssetPrecision { get; set; }
    public int baseCommissionPrecision { get; set; }
    public int quoteCommissionPrecision { get; set; }
    public string[] orderTypes { get; set; }
    public bool icebergAllowed { get; set; }
    public bool ocoAllowed { get; set; }
    public bool quoteOrderQtyMarketAllowed { get; set; }
    public bool allowTrailingStop { get; set; }
    public bool cancelReplaceAllowed { get; set; }
    public bool isSpotTradingAllowed { get; set; }
    public bool isMarginTradingAllowed { get; set; }
    public Filter[] filters { get; set; }
    public string[] permissions { get; set; }
    public string defaultSelfTradePreventionMode { get; set; }
    public string[] allowedSelfTradePreventionModes { get; set; }
}

public class Filter
{
    public string filterType { get; set; }
    public string minPrice { get; set; }
    public string maxPrice { get; set; }
    public string tickSize { get; set; }
    public string minQty { get; set; }
    public string maxQty { get; set; }
    public string stepSize { get; set; }
    public string minNotional { get; set; }
    public bool applyToMarket { get; set; }
    public int avgPriceMins { get; set; }
    public int limit { get; set; }
    public int minTrailingAboveDelta { get; set; }
    public int maxTrailingAboveDelta { get; set; }
    public int minTrailingBelowDelta { get; set; }
    public int maxTrailingBelowDelta { get; set; }
    public string bidMultiplierUp { get; set; }
    public string bidMultiplierDown { get; set; }
    public string askMultiplierUp { get; set; }
    public string askMultiplierDown { get; set; }
    public int maxNumOrders { get; set; }
    public int maxNumAlgoOrders { get; set; }
    public string maxPosition { get; set; }
}
