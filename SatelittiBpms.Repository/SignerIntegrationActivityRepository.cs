using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class SignerIntegrationActivityRepository : AbstractRepositoryBase<SignerIntegrationActivityInfo>, ISignerIntegrationActivityRepository
    {
        public SignerIntegrationActivityRepository(DbContext context) : base(context)
        {
        }
    }
}
