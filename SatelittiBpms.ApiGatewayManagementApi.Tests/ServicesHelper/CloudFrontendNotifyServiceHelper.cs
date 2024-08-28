using Amazon.ApiGatewayManagementApi;
using Microsoft.Extensions.Options;
using SatelittiBpms.ApiGatewayManagementApi.Services;
using SatelittiBpms.Options.Models;

namespace SatelittiBpms.ApiGatewayManagementApi.Tests.ServicesHelper
{
    public class CloudFrontendNotifyServiceHelper : CloudFrontendNotifyService
    {
        public CloudFrontendNotifyServiceHelper(IOptions<AwsOptions> awsOptions) : base(awsOptions)
        { }

        public new IAmazonApiGatewayManagementApi CreateApiClient()
        {
            return base.CreateApiClient();
        }
    }
}
