using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealingAdmin.Abstractions.Providers.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task UpdateAsync(T item);
}