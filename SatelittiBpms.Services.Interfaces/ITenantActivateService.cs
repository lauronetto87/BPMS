using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITenantActivateService
    {
        Task<ResultContent> ActivationTenant(ActivationTenantDTO activationTenantDTO);
    }
}
