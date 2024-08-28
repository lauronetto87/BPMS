using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSignerSegmentService : ISignerSegmentService
    {
        public Task<List<Segment>> List(string tenantSubdomain, string signerAccessToken)
        {
            return Task.FromResult(new List<Segment>()
            {
                new Segment(){ Id = 1, Name = "Segment1", Active = true, IntegrationCode = 11 },
                new Segment(){ Id = 2, Name = "Segment2", Active = true, IntegrationCode = 22 },
                new Segment(){ Id = 3, Name = "Segment3", Active = false, IntegrationCode = 33 },
                new Segment(){ Id = 4, Name = "Segment4", Active = true, IntegrationCode = 44 }
            });
        }
    }
}
