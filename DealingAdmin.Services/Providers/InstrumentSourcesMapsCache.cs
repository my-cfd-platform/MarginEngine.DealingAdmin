using DealingAdmin.Abstractions.Providers.Interfaces;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using SimpleTrading.MyNoSqlRepositories.InstrumentSourcesMaps;
using SimpleTrading.QuotesFeedRouter.Abstractions;

namespace DealingAdmin.Services.Providers
{
    public class InstrumentSourcesMapsCache : ICache<IQuoteFeedSource>
    {
        private readonly IMyNoSqlServerDataReader<InstrumentSourcesMapsMyNoSqlTableEntity> _readRepository;
        private const string TableName = "instrument-sources";

        public InstrumentSourcesMapsCache(IMyNoSqlServerDataReader<InstrumentSourcesMapsMyNoSqlTableEntity> readRepository)
        {
            _readRepository = readRepository;
        }
        public InstrumentSourcesMapsCache(IMyNoSqlSubscriber tcpConnection)
        {
            var readRepository =
                new MyNoSqlReadRepository<InstrumentSourcesMapsMyNoSqlTableEntity>(tcpConnection, TableName);
            _readRepository = readRepository;
        }

        public IEnumerable<IQuoteFeedSource> GetAll()
        {
            var partitionKey = InstrumentSourcesMapsMyNoSqlTableEntity.GeneratePartitionKey();
            return _readRepository.Get(partitionKey);
        }

        public IQuoteFeedSource Get(string id)
        {
            var partitionKey = InstrumentSourcesMapsMyNoSqlTableEntity.GeneratePartitionKey();
            var rowKey = InstrumentSourcesMapsMyNoSqlTableEntity.GenerateRowKey(id);
            return _readRepository.Get(partitionKey, rowKey);
        }

    }
}