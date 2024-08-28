using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class TaskSignerRepository : AbstractRepositoryBase<TaskSignerInfo>, ITaskSignerRepository
    {
        public TaskSignerRepository(DbContext context) : base(context)
        {
        }
    }
}
