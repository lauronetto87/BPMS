using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Utilities.Http
{
    public interface IHttpClientCustom : IDisposable
    {
        TimeSpan Timeout { get; set; }
        Task<string> GetStringAsync(string requestUri);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
