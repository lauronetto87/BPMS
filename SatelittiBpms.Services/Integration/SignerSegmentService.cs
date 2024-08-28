using Microsoft.Extensions.Options;
using Satelitti.Options;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Utilities.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration
{
    public class SignerSegmentService : SignerServiceBase, ISignerSegmentService
    {
        public SignerSegmentService(
            IHttpClientCustom httpClient,
            IOptions<SuiteOptions> suiteOptions,
            IOptions<SignerOptions> signerOptions) : base(httpClient, suiteOptions.Value, $"{signerOptions.Value.BasePath}{signerOptions.Value.SegmentIntegrationPath}")
        { }

        public async Task<List<Segment>> List(string tenantSubdomain, string signerAccessToken)
        {
            return await base.List<List<Segment>>(tenantSubdomain, signerAccessToken);
        }
    }
}
