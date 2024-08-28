using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Satelitti.Authentication.Context.Service;
using Satelitti.Options;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Authentication.Context
{
    public class MockBpmsContextDataService : MockContextDataService<UserInfo>
    {
        public MockBpmsContextDataService(IOptions<SuiteOptions> mockOptions, IHttpContextAccessor httpContextAccessor) : base(mockOptions, httpContextAccessor)
        { }
    }
}
