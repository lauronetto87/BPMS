using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Utilities.Http
{
    [ExcludeFromCodeCoverage]
    public class HttpClientCustom : IHttpClientCustom
    {
        private readonly HttpClient _httpClient;
        public TimeSpan Timeout
        {
            get { return _httpClient.Timeout; }
            set { _httpClient.Timeout = value; }
        }

        public HttpClientCustom()
        {
            _httpClient = new HttpClient();
        }

        public Task<string> GetStringAsync(string requestUri)
        {
            return _httpClient.GetStringAsync(requestUri);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
