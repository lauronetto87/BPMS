using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Options;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Utilities.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration
{
    public class TenantAuthService : ITenantAuthService
    {
        private readonly IHttpClientCustom _httpClient;
        private readonly SuiteOptions _suiteOptions;
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ILogger<SuiteUserService> _logger;
        

        public TenantAuthService(
            IHttpClientCustom httpClient,
            IOptions<SuiteOptions> suiteOptions,
            IContextDataService<UserInfo> contextDataService,
            ILogger<SuiteUserService> logger)
        {
            _httpClient = httpClient;
            _suiteOptions = suiteOptions.Value;
            _contextDataService = contextDataService;
            _logger = logger;            
        }

        public async Task<TenantAuthViewModel> GetTenantAuth(TenantAuthFilter tenantAuthFilter)
        {
            if (string.IsNullOrEmpty(tenantAuthFilter.TenantAccessKey))
                throw new ArgumentException("AccessKey cannot be null or empty", tenantAuthFilter.TenantAccessKey);

            if (string.IsNullOrEmpty(tenantAuthFilter.TenantSubDomain))
                throw new ArgumentException("SubDomain cannot be null or empty", tenantAuthFilter.TenantSubDomain);

            return await RequestTenantAuth(tenantAuthFilter);
        }

        private async Task<TenantAuthViewModel> RequestTenantAuth(TenantAuthFilter tenantAuthFilter)
        {
            var url = string.Format($"{_suiteOptions.UrlBase}{_suiteOptions.Authentication.SuiteUrlGetTenant}", tenantAuthFilter.TenantSubDomain);

            using var req = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Service",tenantAuthFilter.TenantAccessKey)
                }
            };
            using var response = await _httpClient.SendAsync(req);
            var responseUserList = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(responseUserList);

            return JsonConvert.DeserializeObject<TenantAuthViewModel>(responseUserList);
        }
    }
}
