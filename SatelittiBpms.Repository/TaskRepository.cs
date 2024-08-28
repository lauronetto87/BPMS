using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SatelittiBpms.Data.Extensions;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class TaskRepository : AbstractRepositoryBase<TaskInfo>, ITaskRepository
    {
        private readonly IRoleUserRepository _roleUserRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IProcessVersionRepository _processVersionRepository;
        private readonly IFieldValueRepository _fieldValueRepository;

        public TaskRepository(
            IRoleUserRepository roleUserRepository,
            IActivityRepository activityRepository,
            IProcessVersionRepository processVersionRepository,
            IFieldValueRepository fieldValueRepository,
            DbContext context) : base(context)
        {
            _roleUserRepository = roleUserRepository;
            _activityRepository = activityRepository;
            _processVersionRepository = processVersionRepository;
            _fieldValueRepository = fieldValueRepository;
        }

        public override async Task<TaskInfo> Get(int id)
        {
            return await _dbSet.Include(t => t.Activity).FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async override Task<TaskInfo> GetByIdAndTenantId(int id, long tenantId)
        {
            var query = GetByTenantIncludingRelationship(tenantId);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        private IQueryable<TaskInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId)
                .Include(x => x.Activity).ThenInclude(p => p.ProcessVersion)
                .Include(u => u.Executor)
                .Include(f => f.Flow).ThenInclude(pv => pv.ProcessVersion)
                .Include(ac => ac.Activity).ThenInclude(x => x.ActivityUser).ThenInclude(acOpt => acOpt.ActivityUsersOptions)
                .Include(r => r.Activity).ThenInclude(x => x.ActivityUser).ThenInclude(r => r.Role)
                .Include(x => x.Activity).ThenInclude(x => x.ActivityFields).ThenInclude(x => x.Field)
                .Include(x => x.FieldsValues).ThenInclude(x => x.Field)
                .Include(x => x.FieldsValues).ThenInclude(x => x.FieldValueFiles).ThenInclude(x => x.TaskSignerFile.TaskSigner);
        }

        public IQueryable<TaskInfo> ListByTenantFilters(TaskFilterDTO filters, int userId)
        {
            var roleIds = _roleUserRepository.GetQuery(x => x.UserId == userId).Select(x => x.RoleId).ToArray();

            return GetByTenantIncludingRelationship(filters.GetTenantId())
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.BeginDate.HasValue,
                    x => x.CreatedDate >= filters.CreationDateRange.BeginDate.Value.Date.AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.EndDate.HasValue,
                    x => x.CreatedDate < filters.CreationDateRange.EndDate.Value.Date.AddDays(1).AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.FinalizedDateRange != null && filters.FinalizedDateRange.BeginDate.HasValue,
                    x => x.FinishedDate >= filters.FinalizedDateRange.BeginDate.Value.Date.AddMinutes(filters.FinalizedDateRange.TimezoneOffset.Value))
                .WhereIf(filters.FinalizedDateRange != null && filters.FinalizedDateRange.EndDate.HasValue,
                    x => x.FinishedDate < filters.FinalizedDateRange.EndDate.Value.Date.AddDays(1).AddMinutes(filters.FinalizedDateRange.TimezoneOffset.Value))
                .WhereIf(userId > 0,
                                    x => x.ExecutorId == userId || (x.ExecutorId == null && roleIds.Contains(x.Activity.ActivityUser.RoleId ?? 0)))
                .WhereIf(filters.ProcessVersionId != null && filters.ProcessVersionId.Count > 0, x => filters.ProcessVersionId.Contains(x.Activity.ProcessVersionId));
        }

        public TaskInfo GetDetailsById(int id)
        {
            var task = GetQuery(id)
                .Include(x => x.Activity.ActivityUser).ThenInclude(r => r.Role)
                .Include(x => x.Executor)
                .Include(x => x.SignerTasks)
                .Include(x => x.FieldsValues).ThenInclude(f => f.Field)
                .Include(x => x.FieldsValues).ThenInclude(f => f.FieldValueFiles).ThenInclude(fu => fu.UploadedFieldValue)                
                .Include(x => x.FieldsValues).ThenInclude(f => f.FieldValueFiles).ThenInclude(fu => fu.TaskSignerFile)                
                .Include(x => x.Flow.Tasks).ThenInclude(f => f.Activity.ActivityUser)
                .Include(x => x.Flow.Tasks).ThenInclude(f => f.Activity.ActivityFields)
                .Include(x => x.Flow.ProcessVersion.Activities).ThenInclude(a => a.ActivityUser)
                .First();
            SetUpdatedFileValues(task);
            return task;
        }

        private void SetUpdatedFileValues(TaskInfo task)
        {
            foreach (var item in task.FieldsValues)
            {
                if (item.Field.Type == Models.Enums.FieldTypeEnum.FILE)
                {
                    var newValue = new JArray();
                    foreach (var fieldFile in item.FieldValueFiles)
                    {
                        var fieldValueItem = new JObject
                        {
                            { "key", new JValue(fieldFile.Key) },
                            { "size", new JValue(fieldFile.Size) },
                            { "type", new JValue(fieldFile.Type) },
                            { "originalName", new JValue(fieldFile.Name) },
                        };
                        newValue.Add(fieldValueItem);
                    }
                    item.FieldValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
                }
            }
        }
    }
}
