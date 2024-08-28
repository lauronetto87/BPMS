using SatelittiBpms.Models.Integration.Signer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces.Integration
{
    public interface ISignerSubscriberTypeService
    {
        Task<List<SubscriberTypeDescriptionListItem>> List(string tenantSubdomain, string signerAccessToken);
    }
}
