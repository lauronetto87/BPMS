using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class UserService : AbstractServiceBase<UserDTO, UserInfo, IUserRepository>, IUserService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IRoleUserService _roleUserService;
        private readonly ISuiteUserService _suiteUserService;

        public UserService(
            IUserRepository repository,
            ISuiteUserService suiteUserService,
            IMapper mapper,
            IRoleUserService roleUserService,
            IContextDataService<UserInfo> contextDataService) : base(repository, mapper)
        {
            _roleUserService = roleUserService;
            _contextDataService = contextDataService;
            _suiteUserService = suiteUserService;
        }

        public async Task<ResultContent> Disable(int id)
        {
            var infoValue = await _repository.Get(id);

            if (infoValue != null)
            {
                infoValue.Enable = false;
                await _repository.Update(infoValue);

                await _roleUserService.RemoveUserDefaultRole((int)infoValue.TenantId, infoValue.Id);

                return Result.Success();
            }
            else
            {
                throw new InvalidOperationException(ExceptionCodes.ENTITY_TO_UPDATE_NOT_FOUND);
            }
        }

        public async Task<ResultContent> Disable(List<int> info)
        {
            foreach (int id in info)
            {
                try
                {
                    await this.Disable(id);
                }
                catch (Exception) { }
            }

            return Result.Success();
        }

        public override async Task<ResultContent> Update(int id, UserDTO info)
        {
            var infoValue = await _repository.Get(id);

            if (infoValue != null)
            {
                infoValue.Enable = info.Enable;
                infoValue.Type = info.Type;
                if (info.Timezone != null) infoValue.Timezone = info.Timezone;

                await _repository.Update(infoValue);

                if (info.Enable == true)
                {
                    await _roleUserService.InsertUserDefaultRole((int)infoValue.TenantId, infoValue.Id);
                }
                else
                {
                    await _roleUserService.RemoveUserDefaultRole((int)infoValue.TenantId, infoValue.Id);
                }
            }
            else
            {
                await Insert(info);
            }

            return Result.Success();
        }

        public async Task<ResultContent<UserInfo>> Get(int userId, int tenantId)
        {
            return Result.Success<UserInfo>(await _repository.GetByIdAndTenantId(userId, tenantId));
        }

        public ResultContent<List<int>> ListIdsByTenant()
        {
            var context = _contextDataService.GetContextData();

            return Result.Success(_repository.GetQuery(x => x.TenantId == context.Tenant.Id && x.Enable)
                    .Select(x => x.Id)
                    .ToList());
        }

        public async Task<IList<SuiteUserViewModel>> ListUsersSuite()
        {
            List<int> usersBPMS = new();
            var result = ListIdsByTenant();
            if (result.Success)
                usersBPMS = result.Value;

            return usersBPMS.Any() ? await _suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = _contextDataService.GetContextData().SuiteToken, InUserIds = usersBPMS }) :
               await Task.FromResult(new List<SuiteUserViewModel>());
        }

        public override async Task<ResultContent<int>> Insert(UserDTO userDTO)
        {
            try
            {
                var context = _contextDataService.GetContextData();

                if (userDTO.TenantId == null)
                {
                    userDTO.TenantId = context.Tenant.Id;
                }

                if (userDTO.Timezone == null)
                {
                    userDTO.Timezone = context.Tenant.Timezone;
                }

                var resultContent = await base.Insert(userDTO);

                if (userDTO.Enable == true)
                {
                    await _roleUserService.InsertUserDefaultRole((int)userDTO.TenantId, userDTO.Id);
                }

                return resultContent;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.InnerException.Message);
            }
        }

        public async Task<SuiteUserViewModel> GetUsersSuite(int id)
        {
            var context = _contextDataService.GetContextData();

            List<int> usersBPMS = new();
            usersBPMS.Add(context.User.Id);

            var result = await _suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = _contextDataService.GetContextData().SuiteToken, InUserIds = usersBPMS });

            SuiteUserViewModel user = result.FirstOrDefault();

            return user;
        }
    }
}
