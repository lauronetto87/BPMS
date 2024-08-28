using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.ViewModel;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces.Integration
{
    public interface ITenantAuthService
    {
        Task<TenantAuthViewModel> GetTenantAuth(TenantAuthFilter tenantAuthFilter);
    }
}
