using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockTenantAuthService : ITenantAuthService
    {
        public async Task<TenantAuthViewModel> GetTenantAuth(TenantAuthFilter tenantAuthFilter)
        {
            var tenantAuthService = new TenantAuthViewModel
            {
                Id = 55,
                SubDomain = "bmpslocal",
                Zone = "America/Sao_Paulo",
                Timezone= -10800,
                Customizable = false,
                SystemName = "satelitti",
                AccessKey = "6a241a74-9e02-44c7-bcb9-75a203c62f3a",
                AuthenticatedByAccessKey = false,
                Name = "TenantMock",
                Print_id = false
            };

            return await Task.FromResult(tenantAuthService);
        }
    }
}
