using Microsoft.AspNetCore.Http;
using Satelitti.Authentication.Context.Service;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Authentication.Context
{
    public class BpmsContextDataService : ContextDataService<UserInfo>
    {
        public BpmsContextDataService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        { }
    }
}
