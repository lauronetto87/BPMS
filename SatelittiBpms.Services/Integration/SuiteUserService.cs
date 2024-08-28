using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Options;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Utilities.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration
{
    public class SuiteUserService : ISuiteUserService
    {
        private readonly IHttpClientCustom _httpClient;
        private readonly SuiteOptions _suiteOptions;
        private readonly IContextDataService<UserInfo> _contextDataService;

        public SuiteUserService(
            IHttpClientCustom httpClient,
            IOptions<SuiteOptions> suiteOptions,
            IContextDataService<UserInfo> contextDataService)
        {
            _httpClient = httpClient;
            _suiteOptions = suiteOptions.Value;
            _contextDataService = contextDataService;
        }

        public async Task<IList<SuiteUserViewModel>> ListWithContext(SuiteUserListFilter suiteUserListFilter)
        {
            if (string.IsNullOrEmpty(suiteUserListFilter.SuiteToken))
                throw new ArgumentException("Value cannot be null or empty", suiteUserListFilter.SuiteToken);

            var contextData = _contextDataService.GetContextData();
            suiteUserListFilter.TenantSubDomain = contextData.SubDomain;
            suiteUserListFilter.AuthorizationType = AuthorizationTypeEnum.Token;
            return await List(suiteUserListFilter);
        }

        public async Task<IList<SuiteUserViewModel>> ListWithoutContext(SuiteUserListFilter suiteUserListFilter)
        {
            if (string.IsNullOrEmpty(suiteUserListFilter.TenantAccessKey))
                throw new ArgumentException("Value cannot be null or empty", suiteUserListFilter.TenantAccessKey);
            if (string.IsNullOrEmpty(suiteUserListFilter.TenantSubDomain))
                throw new ArgumentException("Value cannot be null or empty", suiteUserListFilter.TenantSubDomain);

            suiteUserListFilter.AuthorizationType = AuthorizationTypeEnum.Service;
            return await List(suiteUserListFilter);
        }

        private async Task<IList<SuiteUserViewModel>> List(SuiteUserListFilter suiteUserListFilter)
        {
            var url = string.Format($"{_suiteOptions.UrlBase}{_suiteOptions.UrlUserList}", suiteUserListFilter.TenantSubDomain);

            var filters = new SuiteUserFiltersDTO
            {
                SelectAll = false,
                Filter = "",
                In = suiteUserListFilter.InUserIds != null && suiteUserListFilter.InUserIds.Any() ? suiteUserListFilter.InUserIds : null
            };

            using HttpRequestMessage req = new(HttpMethod.Post, url)
            {
                Headers = {
                    Authorization = new AuthenticationHeaderValue(Enum.GetName(suiteUserListFilter.AuthorizationType),
                                                        suiteUserListFilter.AuthorizationType == AuthorizationTypeEnum.Token ?
                                                            suiteUserListFilter.SuiteToken :
                                                            suiteUserListFilter.TenantAccessKey)
                },
                Content = new StringContent(JsonConvert.SerializeObject(filters), Encoding.UTF8, "application/json")
            };
            using var response = await _httpClient.SendAsync(req);
            var responseUserList = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(responseUserList);

            return JsonConvert.DeserializeObject<IList<SuiteUserViewModel>>(responseUserList);
        }
    }
}
