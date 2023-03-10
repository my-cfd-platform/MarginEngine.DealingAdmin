using DealingAdmin.Abstractions;
using DealingAdmin.Abstractions.Models;
using DealingAdmin.Abstractions.Models.BidAskSb;
using MyServiceBus.Sdk;
using SimpleTrading.Abstraction.BidAsk;

namespace DealingAdmin.Services
{
    public class PriceRetriever : IPriceRetriever
    {
        private readonly IBidAskCache _bidAskCache;
        private readonly MyServiceBusSubscriberBatchWithoutVersion<BidAskSb> _bidAskSubscriber;
        private readonly MyServiceBusSubscriberBatchWithoutVersion<UnfilteredBidAskSb> _unfilteredBidAskSubscriber;

        private readonly PriceAggregator priceAggregator = new();

        public PriceRetriever(
            IBidAskCache bidAskCache,
            MyServiceBusSubscriberBatchWithoutVersion<BidAskSb> bidAskSubscriber,
            MyServiceBusSubscriberBatchWithoutVersion<UnfilteredBidAskSb> unfilteredBidAskSubscriber)
        {
            _bidAskCache = bidAskCache;
            _bidAskSubscriber = bidAskSubscriber;
            _unfilteredBidAskSubscriber = unfilteredBidAskSubscriber;

            var bidAsks = _bidAskCache.Get();

            priceAggregator.UpdateBidAskCache(bidAsks);

            _bidAskSubscriber.Subscribe(ProcessBidAsk);

            _unfilteredBidAskSubscriber.Subscribe(ProcessUnfilteredBidAsk);
        }

        private ValueTask ProcessBidAsk(BidAskSb bidAsk)
        {
            priceAggregator.UpdateBidAsk(bidAsk.ToIBidAsk());
            return new ValueTask();
        }

        private ValueTask ProcessUnfilteredBidAsk(UnfilteredBidAskSb bidAsk)
        {
            priceAggregator.UpdateUnfilteredBidAsk(bidAsk.ToUnfilteredIBidAsk());
            return new ValueTask();
        }

        public IEnumerable<BidAskModel> GetAllBidAsks() =>
            priceAggregator.GetBidAskCache().Select(itm => itm.ToBidAskModel());

        public BidAskModel GetBidAsk(string instrumentId) =>
            priceAggregator.GetBidAsk(instrumentId)?.ToBidAskModel();

        public UnfilteredBidAskModel GetUnfilteredBidAsk(string instrumentId, string providerId) =>
            priceAggregator.GetUnfilteredBidAsk(instrumentId, providerId)?.ToUnfilteredBidAskModel();
    }
}