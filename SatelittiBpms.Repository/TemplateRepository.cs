using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class TemplateRepository : AbstractRepositoryBase<TemplateInfo>, ITemplateRepository
    {
        public TemplateRepository(DbContext context) : base(context)
        {
        }

        public override async Task<TemplateInfo> Get(int templateId)
        {
            return await _dbSet.FirstOrDefaultAsync(obj => obj.Id == templateId);
        }
    }
}
