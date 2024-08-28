using Satelitti.Authentication.Service;
using Satelitti.Authentication.Service.Interface;
using SatelittiBpms.Authentication.Services.Interfaces;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using System.Diagnostics.CodeAnalysis;

namespace SatelittiBpms.Authentication.Services
{
    [ExcludeFromCodeCoverage]
    public class AuthUserTokenService : AuthUserTokenService<UserInfo, UserViewModel, IAuthUserService>
    {
        public AuthUserTokenService(IAuthUserService authUserService, IAuthUserSuiteTokenService authUserSuiteService) : base(authUserService, authUserSuiteService)
        { }
    }
}
