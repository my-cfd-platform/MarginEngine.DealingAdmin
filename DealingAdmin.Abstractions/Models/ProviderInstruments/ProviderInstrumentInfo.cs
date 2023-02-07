namespace DealingAdmin.Abstractions.Models.ProviderInstruments;

public class ProviderInstrumentInfo
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public int Digits { get; set; }
    public string Base { get; set; }
    public string Quote { get; set; }
    public string TickSize { get; set; }
}