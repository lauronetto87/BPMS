using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Authentication.Types;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _byPassPaths = new List<string>() { $"{ProjectVariableConstants.BpmsUrlPath}/check", $"{ProjectVariableConstants.BpmsUrlPath}/swagger/index.html",
            $"{ProjectVariableConstants.BpmsUrlPath}/swagger/v1/swagger.json" };

        public AuthenticationMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ISuiteService suiteService,
            IContextDataService<UserInfo> contextDataService,
            IAuthUserTokenService<UserViewModel> authUserTokenService)
        {
            if (context.Request.Method == Satelitti.Authentication.Constants.REQUEST_METHOD_OPTIONS ||
                _byPassPaths.Contains(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var suiteToken = context.User?.Claims?.FirstOrDefault(x => x.Type.Equals(CustomClaimTypes.SUITE_TOKEN))?.Value;
            if (!string.IsNullOrEmpty(suiteToken))
            {
                contextDataService.SetSuiteToken(suiteToken);
            }

            context.Response.Headers.Add(Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS, Constants.Constants.RESPONSE_ACCESS_CONTROL_ALL);

            var contextData = contextDataService.GetContextData();

            if (context.Request.Path.Value.ToLower().Contains($"{ProjectVariableConstants.BpmsUrlPath}/authuser/login"))
            {
                var suiteTokenLogin = await GetBody<LoginBySuiteTokenDTO>(context);
                contextData = contextDataService.SetSuiteToken(suiteTokenLogin.Token);
            }

            var tenant = await suiteService.GetTenantBySubDomain(contextData.SubDomain, contextData.SuiteToken);
            if (tenant != null)
                contextData = contextDataService.SetSuiteTenant(tenant);

            var userIdStr = context.User?.Claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Sid))?.Value;
            if (int.TryParse(userIdStr, out int userId) && userId > 0)
            {
                var user = new UserInfo
                {
                    Id = userId,
                    Enable = true,
                    TenantId = tenant?.Id,
                };
                contextDataService.SetUser(user);
            }

            if (string.IsNullOrEmpty(contextData.SuiteToken))
            {
                await _next(context);
                return;
            }

            var tokenResult = await authUserTokenService.Validate(contextData.SuiteToken);
            if (!tokenResult.Success)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(tokenResult.ToErrorJson());
                return;
            }

            if (userId > 0 && tokenResult?.Value?.User != null)
            {
                var user = new UserInfo
                {
                    Id = userId,
                    Enable = true,
                    TenantId = tenant?.Id,
                    Type = tokenResult.Value.User.Type,
                    Timezone = tokenResult.Value.User.Timezone,
                };
                contextDataService.SetUser(user);
            }

            context.Response.Headers.Add(Constants.Constants.RESPONSE_SET_AUTHORIZATION, tokenResult.Value.Token);
            context.Response.Headers.Add(Constants.Constants.RESPONSE_TIMEZONE, tokenResult.Value.User.Timezone.ToString());

            await _next(context);
        }

        private async Task<T> GetBody<T>(HttpContext context)
        {
            string body;
            context.Request.EnableBuffering();

            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}