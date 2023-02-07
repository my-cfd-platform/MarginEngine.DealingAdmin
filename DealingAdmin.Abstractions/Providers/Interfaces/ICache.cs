using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Providers.Interfaces
{
    public interface ICache<T>
    {
        IEnumerable<T> GetAll();
        T Get(string id);
    }
}