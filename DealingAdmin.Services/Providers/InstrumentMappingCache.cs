using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;

namespace DealingAdmin.Services.Providers
{
    public class InstrumentMappingCache : ICache<IProviderInstrumentMap>
    {
        private readonly IMyNoSqlServerDataReader<ProviderInstrumentEntity> _readRepository;
        private const string TableName = "instrument-mapping";

        public InstrumentMappingCache(IMyNoSqlServerDataReader<ProviderInstrumentEntity> readRepository)
        {
            _readRepository = readRepository;
        }
        public InstrumentMappingCache(IMyNoSqlSubscriber tcpConnection)
        {
            var readRepository =
                new MyNoSqlReadRepository<ProviderInstrumentEntity>(tcpConnection, TableName);
            _readRepository = readRepository;
        }

        public IEnumerable<IProviderInstrumentMap> GetAll()
        {
            var partitionKey = ProviderInstrumentEntity.GeneratePartitionKey();
            return _readRepository.Get(partitionKey);
        }

        public IProviderInstrumentMap Get(string id)
        {
            var partitionKey = ProviderInstrumentEntity.GeneratePartitionKey();
            var rowKey = ProviderInstrumentEntity.GenerateRowKey(id);
            return _readRepository.Get(partitionKey, rowKey);
        }

    }
}