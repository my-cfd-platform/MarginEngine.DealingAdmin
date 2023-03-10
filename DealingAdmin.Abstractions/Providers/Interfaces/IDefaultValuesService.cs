using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IDefaultValuesService
{
    Task<string> GetValueAsync(string key, string suffix = "");
    Task<IEnumerable<string>> GetValuesAsync(string key, string suffix = "");
    Task SetValueAsync(string key, string value, string suffix = "");
    Task SetValuesAsync(string key, IEnumerable<string> values, string suffix = "");
}