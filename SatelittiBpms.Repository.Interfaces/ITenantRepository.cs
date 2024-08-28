using Microsoft.EntityFrameworkCore.Storage;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface ITenantRepository
    {
        void Insert(TenantInfo tenant);
        void Update(TenantInfo tenant);
        TenantInfo Get(int id);
        IDbContextTransaction BeginTransaction();
    }
}
