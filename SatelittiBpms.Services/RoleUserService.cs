using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class RoleUserService : AbstractServiceBase<RoleUserDTO, RoleUserInfo, IRoleUserRepository>, IRoleUserService
    {

        private new readonly IRoleUserRepository _repository;
        private readonly ITenantService _tenantService;

        public RoleUserService(IRoleUserRepository repository, IMapper mapper, ITenantService tenantService) : base(repository, mapper)
        {
            _repository = repository;
            _tenantService = tenantService;
        }

        public async Task<ResultContent> InsertUserDefaultRole(int tenantId, int userId)
        {
            var tenantInfo = _tenantService.Get(tenantId);
            var defaultRoleId = (int)tenantInfo.DefaultRoleId;

            var roleUserInfo = await _repository.GetDefaultByUserAndTenant(tenantId, defaultRoleId, userId);

            if (roleUserInfo == null)
            {
                var roleUser = new RoleUserInfo()
                {
                    TenantId = tenantId,
                    RoleId = defaultRoleId,
                    UserId = userId
                };

                await _repository.Insert(roleUser);
            }

            return Result.Success();
        }

        public async Task<ResultContent> RemoveUserDefaultRole(int tenantId, int userId)
        {
            var tenantInfo = _tenantService.Get(tenantId);
            var defaultRoleId = (int)tenantInfo.DefaultRoleId;

            var roleUserInfo = await _repository.GetDefaultByUserAndTenant(tenantId, defaultRoleId, userId);

            if (roleUserInfo != null)
            {
                await _repository.Delete(roleUserInfo);
            }

            return Result.Success();
        }

        public async Task<List<int>> GetRulesIdByUser(int userId)
        {
            return await Task.Run(() =>
            {
                return _repository.GetQuery(ru => ru.UserId == userId).Select(ru => ru.RoleId).ToList();
            });
        }
    }
}
