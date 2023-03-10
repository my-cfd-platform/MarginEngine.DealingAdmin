using DealingAdmin.Abstractions;
using DealingAdmin.Abstractions.Interfaces;
using DealingAdmin.Abstractions.Models.BidAskSb;
using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Services;
using DealingAdmin.Services.Contracts;
using DealingAdmin.Services.Providers;
using DealingAdmin.Shared.Services;
using DealingAdmin.Validators;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using MyNoSqlServer.DataReader;
using MyPostgreSQL;
using MyServiceBus.Abstractions;
using MyServiceBus.Sdk;
using MyServiceBus.TcpClient;
using ProtoBuf.Grpc.Client;
using Serilog;
using SimpleTrading.Abstraction.BidAsk;
using SimpleTrading.Abstraction.Caches.ActiveOrders;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.Abstraction.Markups;
using SimpleTrading.Abstraction.Markups.TradingGroupMarkupProfiles;
using SimpleTrading.Abstraction.Trading;
using SimpleTrading.Abstraction.Trading.Instruments;
using SimpleTrading.Abstraction.Trading.InstrumentsGroup;
using SimpleTrading.Abstraction.Trading.Profiles;
using SimpleTrading.Abstraction.Trading.Swaps;
using SimpleTrading.Admin.Grpc;
using SimpleTrading.Auth.Grpc;
using SimpleTrading.CandlesHistory.AzureStorage;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.ClientApi.Services;
using SimpleTrading.Common.Abstractions.DefaultValues;
using SimpleTrading.Common.MyNoSql;
using SimpleTrading.Engine.Grpc;
using SimpleTrading.MyNoSqlRepositories;
using SimpleTrading.PersonalData.Grpc;
using SimpleTrading.QuotesFeedRouter.Abstractions;
//using SimpleTrading.ServiceBus.PublisherSubscriber.BidAsk;
//using SimpleTrading.ServiceBus.PublisherSubscriber.UnfilteredBidAsk;
using SimpleTrading.TickHistory.Grpc;
using SimpleTrading.TradeLog.Grpc;
using System;
using System.Collections.Generic;

//using SimpleTrading.ServiceBus.Contracts;

namespace DealingAdmin
{
    public class LiveDemoServices
    {
        public ITradingProfileRepository TradingProfileRepository { get; set; }

        public IStDataReader StReader { get; set; }

        public IActiveOrdersCacheReader ActiveOrdersReader { get; set; }

        public ISimpleTradingEngineApi EngineApi { get; set; }

        public ITradingGroupsRepository TradingGroupsRepository { get; set; }

        public ITradingGroupsMarkupProfilesRepository TradingGroupsMarkupProfilesRepository { get; set; }

        public ITradingGroupMarkupProfilePropertiesRepository TradingGroupMarkupProfilePropertiesRepository { get; set; }
    }

    public class LiveDemoServiceMapper
    {
        private readonly Dictionary<bool, LiveDemoServices> _services = new();

        public void InitService(bool isLive, Action<LiveDemoServices> func)
        {
            if (!_services.ContainsKey(isLive))
                _services.Add(isLive, new LiveDemoServices());

            func(_services[isLive]);
        }

        public LiveDemoServices GetContext(bool isLive)
        {
            return _services[isLive];
        }
    }

    public class AdminAppSettings
    {
        public string ChangeBalanceApiKey { get; set; }

        public string AdminCrudApiKey { get; set; }
    }

    public static class ServiceBinder
    {
        private const string AppName = "DealingAdmin";
        private const string EnvInfo = "ENV_INFO";
        private static string AppNameWithEnvMark => $"{AppName}-{GetEnvInfo()}";

        public static void BindServices(this IServiceCollection services, SettingsModel settingsModel)
        {
            services.AddScoped<IUserMessageService, UserMessageService>();

            services.AddScoped<IFileService, FileService>();

            services.AddSingleton(new CandlesServiceSettings()
            {
                CandlesSaveChunkSize = settingsModel.CandlesSaveChunkSize,
                CandlesExpiresMinutes = settingsModel.CandlesExpiresMinutes,
                CandlesExpiresHours = settingsModel.CandlesExpiresHours
            });

            services.AddSingleton(new AdminAppSettings()
            {
                ChangeBalanceApiKey = settingsModel.ChangeBalanceApiKey,
                AdminCrudApiKey = settingsModel.AdminCrudApiKey,
            });

            services.AddScoped<StateManager>();
            services.AddScoped<ICandlesService, CandlesService>();
            services.AddScoped<ISwapProfileUploadService, SwapProfileUploadService>();
            services.AddSingleton<IPriceAggregator, PriceAggregator>();
            services.AddSingleton<IPriceRetriever, PriceRetriever>();
            services.AddSingleton<IAccountTypeFilter, AccountTypeFilter>();
            services.AddSingleton<IQuoteSourceService, QuoteSourceService>();
            services.AddSingleton<ITraderSearchService, TraderSearchService>();
            services.AddSingleton<IAccountNewTradingGroupValidator, AccountNewTradingGroupValidator>();
            services.AddSingleton<ITradingProfileNewInstrumentsValidator, TradingProfileNewInstrumentsValidator>();
        }

        public static void InitLiveDemoManager(this IServiceCollection services, LiveDemoServiceMapper mapper)
        {
            services.AddSingleton(mapper);
        }

        public static MyNoSqlTcpClient BindMyNoSql(
            this IServiceCollection services,
            LiveDemoServiceMapper liveDemoServicesMapper,
            SettingsModel settingsModel)
        {
            var tcpConnection = new MyNoSqlTcpClient(
                () => SimpleTrading.SettingsReader.SettingsReader.ReadSettings<SettingsModel>().PricesMyNoSqlServerReader,
                AppName);

            // ST Services (to be replaced in the future)
            services.AddSingleton<IBidAskCache>(tcpConnection.CreateBidAskMyNoSqlCache());

            #region Data Providers

            #region Price Router Lp Source

            // Repository
            services.AddSingleton<IRepository<IProviderRouterSource>>(
                new ProviderRouterSourceRepository(settingsModel.DictionariesMyNoSqlServerWriter));

            #endregion

            #region Providers Instrument Info

            // Repository
            services.AddSingleton<IProviderInstrumentsInfoService, ProviderInstrumentsInfoService>();

            #endregion

            #region Liquidity Providers

            // Repository
            services.AddSingleton<ILiquidityProviderReader>(
                new LiquidityProviderReader(settingsModel.QuoteFeedRouterUrl));
            // Repository
            services.AddSingleton<IAvailableLiquidityProviders>(
                new AvailableLiquidityProviders(settingsModel.AvailableLiquidityProviders));

            #endregion

            #region Trading Instruments and dependencies

            #region Instruments

            // Cache
            services.AddSingleton<ICache<ITradingInstrument>>(new InstrumentCache(tcpConnection));
            // Repository
            services.AddSingleton<ITradingInstrumentsRepository>(MyNoSqlServerFactory.CreateTradingInstrumentsMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            #endregion

            #region Other
            services.AddSingleton<IInstrumentGroupsRepository>(MyNoSqlServerFactory.CreateInstrumentGroupsMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton<IInstrumentSubGroupsRepository>(MyNoSqlServerFactory.CreateInstrumentSubGroupsMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton(CommonMyNoSqlServerFactory.CreateTradingInstrumentMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));


            #endregion

            #region Instrument Source Map

            services.AddSingleton<ICache<IQuoteFeedSource>>(new InstrumentSourcesMapsCache(tcpConnection));
            services.AddSingleton(MyNoSqlServerFactory.CreateInstrumentSourcesMapsNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));

            #endregion

            #region Instrument Mapping

            // Cache
            services.AddSingleton<ICache<IProviderInstrumentMap>>(new InstrumentMappingCache(tcpConnection));
            // Repository
            services.AddSingleton<IRepository<IProviderInstrumentMap>>(
                new InstrumentMappingRepository(settingsModel.DictionariesMyNoSqlServerWriter));

            #endregion

            #endregion

            #region Default Values

            // Repository
            services.AddSingleton<IRepository<IDefaultValues>>(
                new DefaultValuesRepository(settingsModel.DictionariesMyNoSqlServerWriter));
            
            // Service
            services.AddSingleton<IDefaultValuesService, DefaultValuesService>();

            #endregion

            #endregion


            services.AddSingleton((IDefaultValuesRepository)CommonMyNoSqlServerFactory.CreateDefaultValueMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton((IDefaultLiquidityProviderWriter)CommonMyNoSqlServerFactory.CreateDefaultValueMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton((IDefaultMarkupProfileWriter)CommonMyNoSqlServerFactory.CreateDefaultValueMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton<ISwapScheduleWriter>(MyNoSqlServerFactory.CreateSwapScheduleMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton<ISwapProfileWriter>(MyNoSqlServerFactory.CreateSwapProfileMyNoSqlWriter(
                () => settingsModel.DictionariesMyNoSqlServerWriter));
            services.AddSingleton<IMarkupProfilesRepository>(MyNoSqlServerFactory.CreateMarkupProfilesNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter));

            liveDemoServicesMapper.InitService(true,
               services => services.ActiveOrdersReader = MyNoSqlServerFactory.CreateActiveOrdersCacheReader(tcpConnection, true));

            liveDemoServicesMapper.InitService(false,
               services => services.ActiveOrdersReader = MyNoSqlServerFactory.CreateActiveOrdersCacheReader(tcpConnection, false));


            BindLiveDemoMyNoSql(liveDemoServicesMapper, settingsModel, true);
            BindLiveDemoMyNoSql(liveDemoServicesMapper, settingsModel, false);

            return tcpConnection;
        }

        private static void BindLiveDemoMyNoSql(
            LiveDemoServiceMapper liveDemoServicesMapper,
            SettingsModel settingsModel,
            bool isLive)
        {
            var tradingProfileRepository = MyNoSqlServerFactory.CreateTradingProfilesMyNoSqlRepository(
               () => settingsModel.DictionariesMyNoSqlServerWriter, isLive);

            liveDemoServicesMapper.InitService(isLive, services => services.TradingProfileRepository = tradingProfileRepository);

            var tradingGroupsRepository = MyNoSqlServerFactory.CreateTradingGroupsMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter, isLive);

            liveDemoServicesMapper.InitService(isLive, services => services.TradingGroupsRepository = tradingGroupsRepository);

            var tradingGroupsMarkupProfilesRepository = MyNoSqlServerFactory.CreateTradingGroupsMarkupProfilesNoSqlRepository(
             () => settingsModel.DictionariesMyNoSqlServerWriter, isLive);

            liveDemoServicesMapper.InitService(isLive, services =>
                services.TradingGroupsMarkupProfilesRepository = tradingGroupsMarkupProfilesRepository);

            var tradingGroupMarkupProfilePropertieRepository = MyNoSqlServerFactory.CreateTradingGroupMarkupProfilePropertiesMyNoSqlRepository(
                () => settingsModel.DictionariesMyNoSqlServerWriter, isLive);

            liveDemoServicesMapper.InitService(isLive, services =>
                services.TradingGroupMarkupProfilePropertiesRepository = tradingGroupMarkupProfilePropertieRepository);
        }

        public static void BindPostgresRepositories(
            this IServiceCollection services,
            LiveDemoServiceMapper liveDemoServicesMapper,
            SettingsModel settingsModel)
        {
            var livePostgresConnection = new PostgresConnection(
                settingsModel.PostgresLiveConnectionString,
                AppName,
                settingsModel.PostgresLiveSchema);

            var demoPostgresConnection = new PostgresConnection(
                settingsModel.PostgresDemoConnectionString,
                AppName,
                settingsModel.PostgresDemoSchema);

            var crmDataConnection = new PostgresConnection(
                settingsModel.CrmDataPostgresConnString,
                AppName,
                settingsModel.CrmPostgresSchema);

            services.AddSingleton<ICrmDataReader>(new CrmDataReader(crmDataConnection));

            liveDemoServicesMapper.InitService(true, services => services.StReader = new StDataReader(livePostgresConnection));
            liveDemoServicesMapper.InitService(false, services => services.StReader = new StDataReader(demoPostgresConnection));
        }

        public static void BindRestClients(this IServiceCollection services)
        {
            services.AddSingleton<IRouterRestClient>(new RouterRestClient());
        }

        public static void BindAzureStorage(this IServiceCollection services, SettingsModel settingsModel)
        {
            services.AddSingleton<ICandlesPersistentStorage>(new CandlesPersistentAzureStorage(() =>
                settingsModel.AzureStorageCandlesConnection, () => settingsModel.AzureStorageCandlesConnection));
        }

        public static void BindGrpcServices(
            this IServiceCollection app,
            LiveDemoServiceMapper liveDemoServicesMapper,
            SettingsModel settings)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.CandlesHistoryServiceUrl)
                .CreateGrpcService<ISimpleTradingCandlesHistoryGrpc>());

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.AuthGrpcServiceUrl)
                .CreateGrpcService<IAuthGrpcService>());

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.PersonalDataGrpcServiceUrl)
                .CreateGrpcService<IPersonalDataServiceGrpc>());

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.TradeLogGrpcServiceUrl)
                .CreateGrpcService<ITradeLogGrpcService>());

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.TickHistoryServiceUrl)
                .CreateGrpcService<ITickHistoryServiceGrpc>());

            liveDemoServicesMapper.InitService(true, services => services.EngineApi = GrpcChannel
                .ForAddress(settings.TradingEngineLiveGrpcServerUrl)
                .CreateGrpcService<ISimpleTradingEngineApi>());

            liveDemoServicesMapper.InitService(false, services => services.EngineApi = GrpcChannel
                .ForAddress(settings.TradingEngineDemoGrpcServerUrl)
                .CreateGrpcService<ISimpleTradingEngineApi>());

            app.AddSingleton(GrpcChannel
                .ForAddress(settings.AdminServerServiceUrl)
                .CreateGrpcService<IAdminGrpc>());
        }

        public static MyServiceBusTcpClient BindServiceBus(this IServiceCollection services, SettingsModel settingsModel)
        {
            var appName = AppName;
            #if DEBUG
            appName += "-dev";
            #endif
            var serviceBusTcpClient = new MyServiceBusTcpClient(
                () => settingsModel.PricesMyServiceBusReader,
                AppName);

            services.AddSingleton(new MyServiceBusSubscriberBatchWithoutVersion<UnfilteredBidAskSb>(
                serviceBusTcpClient,
                "unfiltered-bidask",
                appName,
                TopicQueueType.DeleteOnDisconnect,
                false));

            services.AddSingleton(new MyServiceBusSubscriberBatchWithoutVersion<BidAskSb>(serviceBusTcpClient,
                "bidask",
                appName,
                TopicQueueType.DeleteOnDisconnect,
                false));

            services.AddSingleton(new MyServiceBusPublisher<IUnfilteredBidAsk>(
                serviceBusTcpClient,
                "unfiltered-bidask",
                true,
                null
            ));

            services.AddSingleton(new MyServiceBusPublisher<IBidAskSb>(
                serviceBusTcpClient,
                "bidask",
                true,
                null
            ));

            services.AddSingleton(new MyServiceBusPublisher<UpdateCandlesHistoryServiceBusContract>(
                serviceBusTcpClient,
                "update-candles-history",
                true,
                null
            ));
            return serviceBusTcpClient;
        }

        public static void BindLogger(this IServiceCollection services, SettingsModel settings)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("AppName", AppName)
                .Enrich.WithHttpRequestId()
                .Enrich.WithHttpRequestUrl()
                .WriteTo.Seq(settings.SeqServiceUrl)
                .CreateLogger();

            services.AddSingleton(logger);
        }

        private static string GetEnvInfo()
        {
            var info = Environment.GetEnvironmentVariable(EnvInfo);
            if (string.IsNullOrEmpty(info))
                throw new Exception($"Env Variable {EnvInfo} is not found");

            return info;
        }
    }
}