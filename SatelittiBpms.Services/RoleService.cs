using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class RoleService : AbstractServiceBase<RoleDTO, RoleInfo, IRoleRepository>, IRoleService
    {
        private readonly IRoleUserService _roleUserService;
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ITranslateService _translateService;
        private readonly ITenantService _tenantService;

        public RoleService(
            IRoleRepository repository,
            IMapper mapper,
            IRoleUserService roleUserService,
            ITranslateService translateService,
            ITenantService tenantService,
            IContextDataService<UserInfo> contextDataService) : base(repository, mapper)
        {
            _roleUserService = roleUserService;
            _contextDataService = contextDataService;
            _translateService = translateService;
            _tenantService = tenantService;
        }

        public async Task<ResultContent<bool>> VerifyRoleNameExists(string roleName, int? tenantId = null, int? roleId = null)
        {
            if (tenantId == null)
            {
                var contextData = _contextDataService.GetContextData();
                tenantId = contextData.Tenant.Id;
            }

            var count = await _repository.Count(x => x.Name.Equals(roleName) && x.TenantId == tenantId && (roleId == null || x.Id != roleId));
            return Result.Success(count > 0);
        }

        public async Task<ResultContent> InsertWithRelationship(RoleDTO dto)
        {
            var contextData = _contextDataService.GetContextData();
            var exists = await VerifyRoleNameExists(dto.Name, contextData.Tenant.Id);
            if (exists.Value)
            {
                AddErrors(ExceptionCodes.ROLE_NAME_ALREADY_EXISTS, ExceptionCodes.ROLE_NAME_ALREADY_EXISTS);
                return Result.Error(ValidationResult);
            }

            using (var transaction = _repository.BeginTransaction())
            {
                try
                {
                    dto.SetTenantId(contextData.Tenant.Id);
                    var insertResult = await Insert(dto);
                    if (!insertResult.Success)
                        throw new Exception(_translateService.Localize(ExceptionCodes.INSERT_ROLE_ERROR));

                    if (dto.UsersIds != null)
                        await InsertRelationship(dto.UsersIds, contextData.Tenant.Id, insertResult.Value);

                    transaction.Commit();
                    return Result.Success(insertResult.Value);
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    AddErrors(ExceptionCodes.INSERT_ROLE_TRANSACTION_ERROR, e.Message);
                    return Result.Error(ValidationResult);
                }
            }
        }

        public async Task<ResultContent> ListByTenant(Dictionary<string, string> pFilters = null)
        {
            var context = _contextDataService.GetContextData();

            var lstRolesFromTenant = await this.getFilteredRoles(context.Tenant.Id, pFilters);

            return Result.Success(lstRolesFromTenant.Select(x => x.AsViewModel()).ToList());
        }

        public async Task<ResultContent> ListByTenantToConfig(Dictionary<string, string> pFilters = null)
        {
            var context = _contextDataService.GetContextData();
            var tenantInfo = _tenantService.Get(context.Tenant.Id);

            var lstRolesFromTenant = await this.getFilteredRoles(context.Tenant.Id, pFilters);

            return Result.Success(lstRolesFromTenant.Where(x => x.Id != tenantInfo.DefaultRoleId).Select(x => x.AsViewModel()).ToList());
        }

        private async Task<List<RoleInfo>> getFilteredRoles(int tenantId, Dictionary<string, string> pFilters = null)
        {
            if (pFilters == null)
                pFilters = new Dictionary<string, string>();

            if (!pFilters.ContainsKey("TenantId"))
                pFilters.Add("TenantId", tenantId.ToString());

            return await base.ListAsync(pFilters);
        }

        public async Task<ResultContent> DeleteMany(List<int> rolesToDelete)
        {
            foreach (int roleId in rolesToDelete)
            {
                var deleteResult = await Delete(roleId);
                if (!deleteResult.Success)
                    AddErrors(deleteResult.ValidationResult.Errors);
            }

            return ValidationResult.Errors.Any() ? Result.Error(ValidationResult) : Result.Success();
        }

        public async Task<ResultContent<RoleInfo>> Get(int roleId, int tenantId)
        {
            return Result.Success(await _repository.GetByIdAndTenantId(roleId, tenantId));
        }

        public override async Task<ResultContent<RoleInfo>> Get(int roleId)
        {
            var context = _contextDataService.GetContextData();
            return Result.Success(await _repository.GetByIdAndTenantId(roleId, context.Tenant.Id));
        }

        public async Task<ResultContent> GetByTenant(int roleId)
        {
            var context = _contextDataService.GetContextData();
            var role = await _repository.GetByIdAndTenantId(roleId, context.Tenant.Id);
            return Result.Success(role.AsDetailViewModel());
        }

        public async Task<ResultContent> UpdateWithRelationship(int roleId, RoleDTO dto)
        {
            var contextData = _contextDataService.GetContextData();
            var exists = await VerifyRoleNameExists(dto.Name, contextData.Tenant.Id, roleId);
            if (exists.Value)
            {
                AddErrors(ExceptionCodes.ROLE_NAME_ALREADY_EXISTS, ExceptionCodes.ROLE_NAME_ALREADY_EXISTS);
                return Result.Error(ValidationResult);
            }

            using (var transaction = _repository.BeginTransaction())
            {
                try
                {
                    dto.SetTenantId(contextData.Tenant.Id);
                    var roleInfo = await _repository.GetByIdAndTenantId(roleId, contextData.Tenant.Id);
                    RoleInfo currMap = _mapper.Map<RoleInfo>(dto);
                    await _repository.Update(_mapper.Map(currMap, roleInfo));

                    foreach (var roleUser in roleInfo.RoleUsers.ToList())
                        await _roleUserService.Delete(roleUser.Id);
                    await InsertRelationship(dto.UsersIds, contextData.Tenant.Id, roleId);

                    transaction.Commit();
                    return Result.Success();
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    AddErrors(ExceptionCodes.UPDATE_ROLE_TRANSACTION_ERROR, e.Message);
                    return Result.Error(ValidationResult);
                }
            }
        }

        private async Task InsertRelationship(IList<int> lstUsersIds, int tenantId, int roleId)
        {
            foreach (var userId in lstUsersIds)
            {
                await _roleUserService.Insert(new RoleUserDTO()
                {
                    RoleId = roleId,
                    UserId = userId,
                    TenantId = tenantId
                });
            }
        }
    }
}
