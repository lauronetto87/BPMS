using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Linq;

namespace SatelittiBpms.Repository
{
    public class TenantRepository : ITenantRepository
    {
        internal readonly DbSet<TenantInfo> _dbSet;
        internal readonly DbContext _context;

        public TenantRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TenantInfo>();
        }

        public void Insert(TenantInfo info)
        {
            _dbSet.Add(info);
            _context.SaveChanges();
        }

        public TenantInfo Get(int id)
        {
            return _dbSet.FirstOrDefault(x => x.Id == id);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Update(TenantInfo info)
        {
            _dbSet.Update(info);
            _context.SaveChanges();
        }
    }
}
