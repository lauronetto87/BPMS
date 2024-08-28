using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satelitti.Options;
using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Utilities.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class SignerIntegrationRestService : ISignerIntegrationRestService
    {
        private readonly IHttpClientCustom _httpClient;
        private readonly ITenantService _tenantService;
        private readonly SuiteOptions _suiteOptions;

        const string URL_SIGNER_FILE = "/signer/IntegrationEnvelopeDocument";
        const string URL_SIGNER_ENVELOP = "/signer/IntegrationEnvelope";
        const string URL_SIGNER_FILE_DOWNLOAD = "/signer/IntegrationEnvelopeFile";

        internal readonly ILogger<SignerIntegrationRestService> _logger;

        public SignerIntegrationRestService(
            IHttpClientCustom httpClient,
            ITenantService tenantService,
            IOptions<SuiteOptions> suiteOptions,
            ILogger<SignerIntegrationRestService> logger)
        {
            _httpClient = httpClient;
            _tenantService = tenantService;
            _suiteOptions = suiteOptions.Value;
            _logger = logger;
        }

        public async Task<TaskSignerInfo> CreateEnvelope(IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> files, TenantInfo tenantInfo)
        {
            var tenant = _tenantService.Get(tenantInfo.Id);

            var subDomain = tenant.SubDomain;
            var urlBase = _suiteOptions.UrlBase;
            var token = tenant.SignerAccessToken;

            var url = string.Format($"{urlBase}", subDomain);

            var filesInfo = await SendFiles(url, token, integrationEnvelope, files, tenantInfo);

            return await SendRequestCreateEvelope(url, token, integrationEnvelope, filesInfo, tenantInfo);
        }

        private async Task<TaskSignerInfo> SendRequestCreateEvelope(string urlBase, string token, IntegrationEnvelopeDTO integrationEnvelope, List<TaskSignerFileInfo> files, TenantInfo tenantInfo)
        {
            urlBase += URL_SIGNER_ENVELOP;
            using var req = new HttpRequestMessage(HttpMethod.Post, urlBase)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Custom", token)
                },
                Content = new StringContent(JsonConvert.SerializeObject(integrationEnvelope), Encoding.UTF8, "application/json")
            };
            using var response = await _httpClient.SendAsync(req);

            var responseUserList = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(responseUserList);

            return new TaskSignerInfo
            {
                Files = files,
                DateSendEvelope = DateTime.UtcNow,
                Status = TaskSignerStatusEnum.SEND,
                TenantId = tenantInfo.Id,
                EnvelopeId = JObject.Parse(responseUserList).Property("envelopeId").ToObject<int>(),
            };
        }

        private async Task<List<TaskSignerFileInfo>> SendFiles(string urlBase, string token, IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> files, TenantInfo tenantInfo)
        {
            var filesInfo = new List<TaskSignerFileInfo>();
            urlBase += URL_SIGNER_FILE;

            foreach (var file in files)
            {
                var requestBody = new
                {
                    name = file.Name,
                    file = file.Base64Content,
                };
                var requestBodyJson = JsonConvert.SerializeObject(requestBody);
                using var req = new HttpRequestMessage(HttpMethod.Post, urlBase)
                {
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue("Custom", token),
                    },
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };
                using var response = await _httpClient.SendAsync(req);

                var responseUserList = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"SendFile Signer - Url: {urlBase} - Status: {response.StatusCode} - Response: '{responseUserList}'");
                    throw new HttpRequestException(responseUserList);
                }


                integrationEnvelope.Documents ??= new List<IntegrationEnvelopeFileDTO>();

                var idFile = JObject.Parse(responseUserList).Property("id").ToObject<int>();

                integrationEnvelope.Documents.Add(new IntegrationEnvelopeFileDTO
                {
                    Id = idFile,
                });
                filesInfo.Add(new TaskSignerFileInfo
                {
                    SignerId = idFile,
                    FieldValueFileId = file.FieldValueFileId,
                    TenantId = tenantInfo.Id,
                });
            }
            return filesInfo;
        }

        public async Task<FileViewModel> DownloadFile(int signerFileId, SignerEnvelopeFileSuffixEnum fileType, int tenantId)
        {
            var tenant = _tenantService.Get(tenantId);
            var subDomain = tenant.SubDomain;
            var urlBase = _suiteOptions.UrlBase;
            var token = tenant.SignerAccessToken;

            var url = string.Format($"{urlBase}{URL_SIGNER_FILE_DOWNLOAD}", subDomain);

            var paramters = new Dictionary<string, string>
            {
                {"EnvelopeFileSuffix", ((int)fileType).ToString() },
                {"EnvelopeFileId", signerFileId.ToString() },
            };
            url = QueryHelpers.AddQueryString(url, paramters);

            using var req = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Custom", token)
                }
            };
            using var response = await _httpClient.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"URL_DownloadFile: `{url}`. ResponseContent: `{error}`");
            }

            var responseFileSigned = await response.Content.ReadAsByteArrayAsync();
            var streamFileSigned = new MemoryStream(responseFileSigned);

            return new FileViewModel
            {
                Content = streamFileSigned,
                ContentType = response.Content.Headers.ContentType.MediaType,
                FileName = response.Content.Headers.ContentDisposition.FileName,
            };
        }
    }
}
