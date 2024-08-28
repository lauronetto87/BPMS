using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Options.Models;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayManagementApi.Services
{
    public class CloudFrontendNotifyService : IFrontendNotifyService
    {
        private readonly AwsOptions _awsOptions;

        public CloudFrontendNotifyService(IOptions<AwsOptions> awsOptions)
        {
            _awsOptions = awsOptions.Value;
        }

        public async Task Notify(string connectionId, object message)
        {
            var notifyMessage = JsonConvert.SerializeObject(message, Formatting.None);
            var apiClient = CreateApiClient();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(notifyMessage));
            var postConnectionRequest = new PostToConnectionRequest
            {
                ConnectionId = connectionId,
                Data = stream
            };

            try
            {
                await apiClient.PostToConnectionAsync(postConnectionRequest);
            }
            catch (AmazonServiceException e)
            {
                if (e.StatusCode == HttpStatusCode.Gone) // Socket não esta mais conectado
                {
                }
            }
        }

        protected virtual IAmazonApiGatewayManagementApi CreateApiClient()
        {
            return new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = _awsOptions.ApiGateway.WebSocketPostToConnectionUrl
            });
        }
    }
}
