using System.Drawing;
using System.Threading.Tasks;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IDefaultValuesService
{
    Task<string> GetValueAsync(string key, string suffix = "");
    Task SetValueAsync(string key, string value, string suffix = "");
}