using SatelittiBpms.Models.Integration.Signer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces.Integration
{
    public interface ISignerSegmentService
    {
        Task<List<Segment>> List(string tenantSubdomain, string signerAccessToken);
    }
}
