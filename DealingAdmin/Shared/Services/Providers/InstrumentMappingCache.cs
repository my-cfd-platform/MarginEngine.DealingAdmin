using DealingAdmin.Shared.Services.Providers.Interfaces;
using DealingAdmin.Shared.Services.Providers.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using System.Collections.Generic;

namespace DealingAdmin.Shared.Services.Providers
{
    public class InstrumentMappingCache : IInstrumentMappingCache
    {
        private readonly IMyNoSqlServerDataReader<ProviderInstrumentMapMyNoSqlEntity> _readRepository;
        private const string TableName = "instrument-mapping";

        public InstrumentMappingCache(IMyNoSqlServerDataReader<ProviderInstrumentMapMyNoSqlEntity> readRepository)
        {
            _readRepository = readRepository;
        }
        public InstrumentMappingCache(IMyNoSqlSubscriber tcpConnection)
        {
            var readRepository =
                new MyNoSqlReadRepository<ProviderInstrumentMapMyNoSqlEntity>(tcpConnection, TableName);
            _readRepository = readRepository;
        }

        public IEnumerable<IProviderInstrumentMap> GetAll()
        {
            var partitionKey = ProviderInstrumentMapMyNoSqlEntity.GeneratePartitionKey();
            return _readRepository.Get(partitionKey);
        }

        public IProviderInstrumentMap Get(string id)
        {
            var partitionKey = ProviderInstrumentMapMyNoSqlEntity.GeneratePartitionKey();
            var rowKey = ProviderInstrumentMapMyNoSqlEntity.GenerateRowKey(id);
            return _readRepository.Get(partitionKey, rowKey);
        }

    }
}