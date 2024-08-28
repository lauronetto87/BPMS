using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<List<T>> ListAsync();

        Task<T> Get(int id);

        IQueryable<T> GetQuery(int id);

        IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate);

        Task<T> GetByIdAndTenantId(int id, long tenantId);

        IQueryable<T> GetByTenant(long tenantId);

        Task<int> Insert(T info);

        Task Update(T info);

        Task Delete(T info);        

        Task<int> Count(Expression<Func<T, bool>> predicate);

        IDbContextTransaction BeginTransaction();
    }
}
