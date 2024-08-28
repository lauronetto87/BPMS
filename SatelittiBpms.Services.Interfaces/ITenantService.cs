using Microsoft.EntityFrameworkCore.Storage;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITenantService
    {
        TenantInfo Get(int id);
        IDbContextTransaction BeginTransaction();
        void Insert(TenantInfo tenantInfo);
        void Update(TenantInfo tenantInfo);
        ResultContent<bool> SignerIntegrationIsEnable();
    }
}
