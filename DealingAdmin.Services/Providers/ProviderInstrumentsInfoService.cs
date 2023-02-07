using DealingAdmin.Abstractions.Models.ProviderInstruments;
using DealingAdmin.Abstractions.Providers.Interfaces;
using Flurl.Http;
using Newtonsoft.Json;

namespace DealingAdmin.Services.Providers;

public class ProviderInstrumentsInfoService: IProviderInstrumentsInfoService
{
    private Dictionary<string, BinanceProduct> _binanceProductStore;
    private IEnumerable<BinanceSymbol> _binanceSymbolStore;
    private Dictionary<string, IEnumerable<ProviderInstrumentInfo>> _providerInfoStore = new ();
    private const string BinanceApi = "https://api.binance.com/api/v3/exchangeInfo";
    private const string BinanceCoinInfo = "https://www.binance.com/en/markets/coinInfo";

    public async Task<IEnumerable<ProviderInstrumentInfo>> InstrumentsInfo(string providerId)
    {
        switch (providerId.ToLowerInvariant())
        {
            case "binance":
                return await GetBinanceInstrumentsInfo();
            default:
                return new List<ProviderInstrumentInfo>();
        }
    }

    //todo when adding more providers move binance methods to separate service

    private async Task<IEnumerable<ProviderInstrumentInfo>> GetBinanceInstrumentsInfo()
    {
        if (_providerInfoStore.ContainsKey("binance"))
        {
            return _providerInfoStore["binance"];
        }

        var products = await GetBinanceProducts();
        var symbols = await GetBinanceSymbols();
        _providerInfoStore["binance"] = from binanceSymbol
                in symbols
            where products.ContainsKey(binanceSymbol.symbol)

            let product = products[binanceSymbol.symbol]
            select new ProviderInstrumentInfo
            {
                Symbol = binanceSymbol.symbol,
                Name = $"{product.baseAssetName} / {product.quoteName}",
                Digits = product.tickSize.Length - 2,
                Base = binanceSymbol.baseAsset,
                Quote = binanceSymbol.quoteAsset,
                TickSize = product.tickSize
            };
        return _providerInfoStore["binance"];
    }

    private async Task<IEnumerable<BinanceSymbol>> GetBinanceSymbols()
    {
        if (_binanceSymbolStore != null)
            return _binanceSymbolStore;
        var res = await BinanceApi.GetJsonAsync<BinanceInfo>();
        _binanceSymbolStore = res.symbols;
        return _binanceSymbolStore;
    }

    private async Task<Dictionary<string, BinanceProduct>> GetBinanceProducts()
    {
        if (_binanceProductStore != null)
            return _binanceProductStore;
        
        var productsString = await BinanceCoinInfo.GetStringAsync().ConfigureAwait(false);
        //var start = test.IndexOf(, StringComparison.Ordinal);
        productsString = productsString.Split("<script id=\"__APP_DATA\" type=\"application/json\">")[1];
        productsString = productsString.Split("</script>")[0];

        _binanceProductStore = JsonConvert.DeserializeObject<BinanceWebInfo>(productsString).pageData.redux.products.productMap;

        return _binanceProductStore;
    }

    public async Task<Dictionary<string, string>> Instruments(string providerId)
    {
        switch (providerId.ToLowerInvariant())
        {
            case "binance":
                return await BinanceInstruments();
            default:
                return default;
        }
    }

    private async Task<Dictionary<string, string>> BinanceInstruments()
    {
        var info = await GetBinanceProducts();
        var nameDictionary = new Dictionary<string, string>();
        foreach (var (name,product) in info)
        {
            if (!nameDictionary.ContainsKey(product.baseAsset))
            {
                nameDictionary.Add(product.baseAsset, product.baseAssetName);
            }

            if (!nameDictionary.ContainsKey(product.quoteAsset))
            {
                nameDictionary.Add(product.quoteAsset, product.quoteName);
            }
        }

        return nameDictionary;
    }

}