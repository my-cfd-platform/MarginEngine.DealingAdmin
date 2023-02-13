using DealingAdmin.Abstractions.Providers.Interfaces;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using SimpleTrading.Abstraction.Trading.Instruments;
using SimpleTrading.MyNoSqlRepositories.Trading.Instruments;

namespace DealingAdmin.Services.Providers
{
    public class InstrumentCache : ICache<ITradingInstrument>
    {
        private readonly IMyNoSqlServerDataReader<TradingInstrumentMyNoSqlEntity> _readRepository;
        private const string TableName = "instruments";

        public InstrumentCache(IMyNoSqlServerDataReader<TradingInstrumentMyNoSqlEntity> readRepository)
        {
            _readRepository = readRepository;
        }
        public InstrumentCache(IMyNoSqlSubscriber tcpConnection)
        {
            var readRepository =
                new MyNoSqlReadRepository<TradingInstrumentMyNoSqlEntity>(tcpConnection, TableName);
            _readRepository = readRepository;
        }

        public IEnumerable<ITradingInstrument> GetAll()
        {
            var partitionKey = TradingInstrumentMyNoSqlEntity.GeneratePartitionKey();
            return _readRepository.Get(partitionKey);
        }

        public ITradingInstrument Get(string id)
        {
            var partitionKey = TradingInstrumentMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = TradingInstrumentMyNoSqlEntity.GenerateRowKey(id);
            return _readRepository.Get(partitionKey, rowKey);
        }

    }
}