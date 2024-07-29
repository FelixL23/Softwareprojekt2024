using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoPro24Team03.Data
{
    /// <summary>
    /// Abstraction for accessing database.
    /// </summary>
    public interface IRepository<T> : IDisposable
    {
        public Task<List<T>> ToList();
        public Task<T?> Find(int id);
        public Task<bool> Exists(int id);

        public Task Add(T element);
        public Task Update(T element);

        public Task Remove(int id);
        public Task Remove(T element);
    }
}