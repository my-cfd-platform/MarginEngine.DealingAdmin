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

    public async Task SetValueAsync(string key, string value, string suffix = "")
    {
        var entity = new DefaultValuesEntity()
        {
            Id = $"{key}{suffix}",
            Value = value
        };
        await _repository.UpdateAsync(entity);
    }
}
