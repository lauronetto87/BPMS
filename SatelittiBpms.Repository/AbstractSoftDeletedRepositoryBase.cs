using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public abstract class AbstractSoftDeletedRepositoryBase<T> : AbstractRepositoryBase<T> where T : DeletableBaseInfo
    {
        private readonly Func<T, bool> FuncFilterSoftDelete =
             c => c.IsDeleted == false;

        private readonly Expression<Func<T, bool>> FilterSoftDelete =
           c => c.IsDeleted == false;

        public AbstractSoftDeletedRepositoryBase(DbContext context) : base(context)
        {
        }

        public new virtual async Task Delete(T info)
        {
            info.IsDeleted = true;
            await this.Update(info);
        }

        public new virtual async Task<List<T>> ListAsync()
        {
            List<T> list = await base.ListAsync();
            if (list != null)
                return list.Where(FuncFilterSoftDelete).ToList();
            return list;
        }

        public async Task<List<T>> ListDeletedAsync()
        {
            var list = await base.ListAsync();
            if (list != null)
                return list.Where(x => x.IsDeleted == true).ToList();
            return list;
        }

        public override IQueryable<T> GetQuery(int id)
        {
            return base.GetQuery(id).Where(FilterSoftDelete);
        }

        public override IQueryable<T> GetByTenant(long tenantId)
        {
            return base.GetByTenant(tenantId).Where(FilterSoftDelete);
        }

        public virtual IQueryable<T> GetQuery(int id, bool includeIsDeleted)
        {
            if (includeIsDeleted)
                return base.GetQuery(id);
            return GetQuery(id);
        }
    }
}
