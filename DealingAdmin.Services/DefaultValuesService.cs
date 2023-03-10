using DealingAdmin.Abstractions.Providers.Interfaces;
using DealingAdmin.Abstractions.Providers.Models;

namespace DealingAdmin.Services;

public class DefaultValuesService: IDefaultValuesService
{
    private readonly IRepository<IDefaultValues> _repository;
    public DefaultValuesService(IRepository<IDefaultValues> repository)
    {
        _repository = repository;
    }

    public async Task<string> GetValueAsync(string key, string suffix)
    {
        var res = await _repository.GetAsync($"{key}{suffix}");
        return res != null ? res.Value : string.Empty;
    }

    public async Task<IEnumerable<string>> GetValuesAsync(string key, string suffix)
    {
        var res = await _repository.GetAsync($"{key}{suffix}");
        return res is { Values: { } } ? res.Values : new List<string>();
    }

    public async Task SetValueAsync(string key, string value, string suffix = "")
    {
        var entity = new DefaultValuesEntity
        {
            Id = $"{key}{suffix}",
            Value = value
        };
        await _repository.UpdateAsync(entity);
    }

    public async Task SetValuesAsync(string key, IEnumerable<string> values, string suffix = "")
    {
        var entity = new DefaultValuesEntity
        {
            Id = $"{key}{suffix}",
            Values = values
        };
        await _repository.UpdateAsync(entity);
    }
}
