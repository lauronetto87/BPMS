using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Data.Extensions;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Linq;

namespace SatelittiBpms.Repository
{
    public class FlowRepository : AbstractRepositoryBase<FlowInfo>, IFlowRepository
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleUserRepository _roleUserRepository;
        private readonly IFieldValueRepository _fieldValueRepository;

        public FlowRepository(
            IActivityRepository activityRepository,
            IRoleRepository roleRepository,
            IRoleUserRepository roleUserRepository,
            IFieldValueRepository fieldValueRepository,
            DbContext context) : base(context)
        {
            _activityRepository = activityRepository;
            _roleRepository = roleRepository;
            _roleUserRepository = roleUserRepository;
            _fieldValueRepository = fieldValueRepository;
        }

        private IQueryable<FlowInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId)
                .Include(f => f.ProcessVersion).ThenInclude(a => a.Activities).ThenInclude(au => au.ActivityUser).ThenInclude(r => r.Role).ThenInclude(ru => ru.RoleUsers)
                .Include(t => t.Tasks).ThenInclude(fi => fi.FieldsValues).ThenInclude(f => f.Field);

        }

        public IQueryable<FlowInfo> ListByTenantFilters(TaskFilterDTO filters, int[] roleIds, int userId)
        {
            return GetByTenantIncludingRelationship(filters.GetTenantId())
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.BeginDate.HasValue,
                                x => x.CreatedDate >= filters.CreationDateRange.BeginDate.Value.Date.AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.EndDate.HasValue,
                                x => x.CreatedDate < filters.CreationDateRange.EndDate.Value.Date.AddDays(1).AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.FinalizedDateRange != null && filters.FinalizedDateRange.BeginDate.HasValue,
                                x => x.FinishedDate >= filters.FinalizedDateRange.BeginDate.Value.Date.AddMinutes(filters.FinalizedDateRange.TimezoneOffset.Value))
                .WhereIf(filters.FinalizedDateRange != null && filters.FinalizedDateRange.EndDate.HasValue,
                                x => x.FinishedDate < filters.FinalizedDateRange.EndDate.Value.Date.AddDays(1).AddMinutes(filters.FinalizedDateRange.TimezoneOffset.Value))                
                .WhereIf(userId > 0, x =>
                    x.ProcessVersion.Activities.Any(a => // Tarefas que serão executadas (Futuro)
                        (a.ActivityUser.ExecutorType == UserTaskExecutorTypeEnum.REQUESTER && x.RequesterId == userId) ||
                        (a.ActivityUser.ExecutorType == UserTaskExecutorTypeEnum.ROLE && roleIds.Contains(a.ActivityUser.RoleId.Value) ||
                        (a.ActivityUser.ExecutorType == UserTaskExecutorTypeEnum.PERSON && a.ActivityUser.PersonId.Value == userId))
                    ) ||
                    x.Tasks.Any(a => a.ExecutorId == userId) || // Tarefas que já foram executadas e em andamento (Passado e atual)
                    x.RequesterId == userId // Solicitante 
                )
                .WhereIf(filters.ProcessVersionId != null && filters.ProcessVersionId.Count > 0, x => filters.ProcessVersionId.Contains(x.ProcessVersionId))
                .WhereIf(filters.HideFinalized == true, x => x.Status != FlowStatusEnum.FINISHED)
                .WhereIf(filters.HideAlreadyParticipated == true, x => x.Tasks.Count(y => y.ExecutorId == userId && y.FinishedDate != null) == 0);
        }
    }
}
