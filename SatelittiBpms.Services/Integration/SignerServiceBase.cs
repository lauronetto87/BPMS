using Newtonsoft.Json;
using Satelitti.Options;
using SatelittiBpms.Utilities.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration
{
    public class SignerServiceBase
    {
        private readonly string _integrationPath;
        private readonly IHttpClientCustom _httpClient;
        private readonly SuiteOptions _suiteOptions;

        public SignerServiceBase(
            IHttpClientCustom httpClient,
            SuiteOptions suiteOptions,
            string integrationPath)
        {
            _httpClient = httpClient;
            _suiteOptions = suiteOptions;
            _integrationPath = integrationPath;
        }

        public async Task<T> List<T>(string tenantSubdomain, string signerAccessToken)
        {
            var url = string.Format($"{_suiteOptions.UrlBase}{_integrationPath}", tenantSubdomain);

            using HttpRequestMessage req = new(HttpMethod.Get, url)
            {
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Custom", signerAccessToken)
                }
            };
            using var response = await _httpClient.SendAsync(req);
            var responseUserList = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(responseUserList);

            return JsonConvert.DeserializeObject<T>(responseUserList);
        }
    }
}
