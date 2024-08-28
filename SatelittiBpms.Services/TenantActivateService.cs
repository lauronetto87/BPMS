using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class TenantActivateService : FluentValidationServiceBase, ITenantActivateService
    {
        private readonly ITenantService _tenantService;
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ISuiteUserService _suiteUserService;
        private readonly IUserService _userService;
        private readonly ITenantAuthService _tenantAuthService;
        private readonly IRoleService _roleService;

        public TenantActivateService(
            ITenantAuthService tenantAuthService,
            ISuiteUserService suiteUserService,
            IUserService userService,
            ITenantService tenantService,
            IRoleService roleService,
            IContextDataService<UserInfo> contextDataService)
        {
            _tenantService = tenantService;
            _contextDataService = contextDataService;
            _suiteUserService = suiteUserService;
            _userService = userService;
            _tenantAuthService = tenantAuthService;
            _roleService = roleService;
        }

        public async Task<ResultContent> ActivationTenant(ActivationTenantDTO activationTenantDTO)
        {
            try
            {
                var contextData = _contextDataService.GetContextData();
                var tenantAuth = await _tenantAuthService.GetTenantAuth(new TenantAuthFilter { TenantAccessKey = activationTenantDTO.AccessKey, TenantSubDomain = contextData.SubDomain });

                if (tenantAuth.SubDomain != contextData.SubDomain)
                {
                    AddErrors(ExceptionCodes.SUBDOMAIN_DIFFERENT_FROM_INFORMED, ExceptionCodes.SUBDOMAIN_DIFFERENT_FROM_INFORMED);
                    return Result.Error(ValidationResult);
                }

                if (tenantAuth.AccessKey != activationTenantDTO.AccessKey)
                {
                    AddErrors(ExceptionCodes.ACCESSKEY_DIFFERENT_FROM_INFORMED, ExceptionCodes.ACCESSKEY_DIFFERENT_FROM_INFORMED);
                    return Result.Error(ValidationResult);
                }

                var tenants = _tenantService.Get(tenantAuth.Id);
                if (tenants == null)
                {
                    var suiteUsers = await _suiteUserService.ListWithoutContext(new SuiteUserListFilter { TenantAccessKey = tenantAuth.AccessKey, TenantSubDomain = tenantAuth.SubDomain });
                    var suiteUsersAdmin = suiteUsers.Where(x => x.Admin);

                    using (var transaction = _tenantService.BeginTransaction())
                    {
                        try
                        {
                            var tenantInfo = new TenantInfo()
                            {
                                Id = tenantAuth.Id,
                                AccessKey = activationTenantDTO.AccessKey,
                                SubDomain = tenantAuth.SubDomain
                            };

                            _tenantService.Insert(tenantInfo);

                            var roleInsertResult = await _roleService.Insert(new RoleDTO() { TenantId = tenantAuth.Id, Name = "Todos" });

                            tenantInfo.DefaultRoleId = roleInsertResult.Value;

                            _tenantService.Update(tenantInfo);

                            foreach (var item in suiteUsersAdmin)
                            {
                                await _userService.Insert(new UserDTO
                                {
                                    Enable = true,
                                    Type = Models.Enums.BpmsUserTypeEnum.ADMINISTRATOR,
                                    Timezone = -3,
                                    Id = item.Id,
                                    TenantId = tenantAuth.Id
                                });
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            AddErrors(ExceptionCodes.INSERT_TENANTAUTH_ERROR, ex.Message);
                            return Result.Error(ValidationResult);
                        }
                    }

                    return Result.Success(suiteUsersAdmin);
                }

                AddErrors(ExceptionCodes.TENANT_ALREADY_INFORMED, ExceptionCodes.TENANT_ALREADY_INFORMED);
                return Result.Error(ValidationResult);
            }
            catch (ArgumentException argExc)
            {
                AddErrors(ExceptionCodes.INSERT_TENANTAUTH_ERROR, argExc.Message);
                return Result.Error(ValidationResult);
            }
            catch (HttpRequestException requestExc)
            {
                AddErrors(ExceptionCodes.INSERT_TENANTAUTH_ERROR, requestExc.Message);
                return Result.Error(ValidationResult);
            }
        }
    }
}
