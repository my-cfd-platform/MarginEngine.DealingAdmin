using System.Collections.Generic;
using DealingAdmin.Abstractions.Providers.Interfaces;

namespace DealingAdmin.Abstractions.Models.LpSettings;

public class ProviderRouterSourceViewModel
{
    public string LiquidityProviderId { get; set; }
    public string RemoteUrl { get; set; }
    public List<string> InstrumentIds { get; set; }

    #region Editor
    public bool BeingEdited { get; set; } = false;
    public string PreEditRemoteUrl { get; set; }
    #endregion


    public static ProviderRouterSourceViewModel Create(IProviderRouterSource src)
    {
        return new ProviderRouterSourceViewModel
        {
            LiquidityProviderId = src.LpId,
            RemoteUrl = src.RemoteUrl,
            InstrumentIds = src.InstrumentIds
        };
    }
}