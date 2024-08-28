using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class FieldValueFileRepository : AbstractRepositoryBase<FieldValueFileInfo>, IFieldValueFileRepository
    {
        public FieldValueFileRepository(DbContext context) : base(context)
        {
        }
    }
}
