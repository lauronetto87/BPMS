using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class FieldRepository : AbstractRepositoryBase<FieldInfo>, IFieldRepository
    {
        public FieldRepository(DbContext context) : base(context)
        {
        }
    }
}
