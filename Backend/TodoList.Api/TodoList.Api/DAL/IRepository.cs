using System;
using System.Collections.Generic;

namespace TodoList.Api.DAL
{
    public interface IRepository<T>
    {
        T Get(Guid id);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        IEnumerable<T> Get();
        void SaveChanges();
    }
}
