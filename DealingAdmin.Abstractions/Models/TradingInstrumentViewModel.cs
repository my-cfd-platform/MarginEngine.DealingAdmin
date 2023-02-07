using SimpleTrading.Abstraction.Trading.Instruments;
using System.Collections.Generic;
using System.Linq;

namespace DealingAdmin.Abstractions.Models
{
    public class TradingInstrumentViewModel : TradingInstrumentModel
    {
        public string LiquidityProviderId { get; set; }

        public bool DefaultLiquidityProvider { get; set; } = true;

        #region Instrument Mapping Modal
        public bool IsNew { get; init; } = true;
        public bool IsModified { get; set; } = false;
        #endregion

        #region Helpers
        public bool IsSelected { get; set; } = false;
        public List<TradingInstrumentDayOffModel> DaysOffSave { get; set; }
        public Dictionary<string, string> LpSymbolDictionary { get; set; } = new ();
        public Dictionary<string, string> LpSymbolDictionarySave { get; set; }
        public Dictionary<string, string> LpSymbolDictionaryDelete { get; set; } = new();
        #endregion

        public static TradingInstrumentViewModel Create(
            ITradingInstrument src,
            string liquidityProviderId,
            bool isDefaultLiquidityProvider)
        {
            return new TradingInstrumentViewModel
            {
                Id = src.Id,
                Name = src.Name,
                Digits = src.Digits,
                Base = src.Base,
                Quote = src.Quote,
                TickSize = src.TickSize,
                SwapScheduleId = src.SwapScheduleId,
                GroupId = src.GroupId,
                SubGroupId = src.SubGroupId,
                Weight = src.Weight,
                DayTimeout = src.DayTimeout,
                NightTimeout = src.NightTimeout,
                TradingDisabled = src.TradingDisabled,
                DaysOff = src.DaysOff.Select(TradingInstrumentDayOffModel.Create).ToList(),
                MarginCallPercent = src.MarginCallPercent,
                LiquidityProviderId = liquidityProviderId,
                DefaultLiquidityProvider = isDefaultLiquidityProvider,
                #region Helpers
                IsNew = false
                #endregion
            };
        }
    }
}