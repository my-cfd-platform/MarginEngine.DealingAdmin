using SimpleTrading.Abstraction.Trading.Instruments;

namespace DealingAdmin.Abstractions.Models.LpSettings;

public class InstrumentMappingViewModel
{
    public string LiquidityProviderId { get; set; }
    public bool DefaultLiquidityProvider { get; set; } = true;
    public string InstrumentId { get; set; }
    public string InstrumentName { get; set; }
    public string GroupId { get; set; }
    public string Base { get; set; }
    public string Quote { get; set; }
    public string MapName { get; set; }

    #region Editor
    public bool BeingEdited { get; set; } = false;
    public string PreEditMapName { get; set; }
    #endregion


    public static InstrumentMappingViewModel Create(
        ITradingInstrument src,
        string liquidityProviderId,
        bool isDefaultLiquidityProvider, string mapName)
    {
        return new InstrumentMappingViewModel
        {
            InstrumentId = src.Id,
            InstrumentName = src.Name,
            Base = src.Base,
            Quote = src.Quote,
            GroupId = src.GroupId,
            LiquidityProviderId = liquidityProviderId,
            DefaultLiquidityProvider = isDefaultLiquidityProvider,
            MapName = mapName
        };
    }
}