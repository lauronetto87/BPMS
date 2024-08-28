using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.VersionNormalization.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.VersionNormalization.Repository
{
    public class VersionNormalizationRepository : IVersionNormalizationRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<VersionNormalizationInfo> _dbSet;

        public VersionNormalizationRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<VersionNormalizationInfo>();
        }

        public void Insert(VersionNormalizationInfo info)
        {
            _dbSet.Add(info);
            _context.SaveChanges();
        }

        public List<VersionNormalizationInfo> ListAll()
        {
            return _dbSet.ToList();
        }
    }
}
