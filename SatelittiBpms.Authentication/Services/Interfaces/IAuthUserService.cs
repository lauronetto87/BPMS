using Satelitti.Authentication.Service.Interface;
using SatelittiBpms.Models.ViewModel;

namespace SatelittiBpms.Authentication.Services.Interfaces
{
    public interface IAuthUserService : IResolverUser<UserViewModel>
    {
    }
}
