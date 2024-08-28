using Microsoft.Extensions.Options;
using Satelitti.Authentication.Authorization;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Model;
using Satelitti.Authentication.Result;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Options;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Authentication.Services.Interfaces;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Services
{
    public class AuthUserService : IAuthUserService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ITokenService<GenerateTokenParameters> _tokenService;
        private readonly IUserService _userService;
        private readonly SuiteAuthenticationOptions _authenticationOptions;

        public AuthUserService(
             ITokenService<GenerateTokenParameters> tokenService,
             IUserService userService,
             IOptions<SuiteAuthenticationOptions> authenticationOptions,
             IContextDataService<UserInfo> contextDataService)
        {
            _contextDataService = contextDataService;
            _tokenService = tokenService;
            _userService = userService;
            _authenticationOptions = authenticationOptions.Value;
        }

        public async Task<ResultContent<AuthTokenWithUser<UserViewModel>>> ResolveProductUser(SuiteUserAuth suiteUser)
        {
            UserInfo bpmsUser = null;
            if (suiteUser.Admin)
                bpmsUser = UserInfo.AsBpmsAdminUser(suiteUser);
            else
            {
                var result = await _userService.Get(suiteUser.Id, suiteUser.Tenant);
                if (result.Success)
                    bpmsUser = result.Value;
            }

            if (bpmsUser == null || !bpmsUser.Enable)
            {
                return Result.Error<AuthTokenWithUser<UserViewModel>>(ResultErrors.ERROR_AUTHENTICATION_INVALIDUSERORINACTIVE);
            }

            var authToken = _tokenService.GenerateToken(
                GenerateTokenParameters
                    .AsBpmsUserParameter(
                        suiteUser,
                        bpmsUser,
                        _authenticationOptions.TokenLifetimeInMinutes
                    )
            );

            _contextDataService.SetUser(bpmsUser);

            UserViewModel userViewModel = UserViewModel.Create(suiteUser, bpmsUser);

            var authTokenWithUser = new AuthTokenWithUser<UserViewModel>()
            {
                Token = authToken.Token,
                ExpiresIn = authToken.ExpiresIn,
                User = userViewModel
            };

            return Result.Success(authTokenWithUser);
        }
    }
}
