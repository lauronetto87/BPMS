using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SatelittiBpms.ApiGatewayMock.Interfaces;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Utilities.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayMock.Services
{
    public class DefaultWebSocketService : IDefaultWebSocketService
    {
        private readonly IHttpClientCustom _httpClient;
        Dictionary<string, WebSocket> dicWebSockets = new Dictionary<string, WebSocket>();

        public DefaultWebSocketService(IHttpClientCustom httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Connect(HttpContext context, Func<Task> next)
        {
            if (context.Request.Path == ProjectVariableConstants.BpmsUrlPath)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        string connectionId = CreateWebSocketId();
                        dicWebSockets.Add(connectionId, webSocket);
                        await ReceiveMessage(connectionId, webSocket);
                        dicWebSockets.Remove(connectionId);
                    }
                }
                else
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
                await next();
        }

        private async Task ReceiveMessage(string connectionId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    continue;

                var message = JObject.Parse(Encoding.UTF8.GetString(buffer));
                if (message.ContainsKey("action"))
                {
                    string a = message.GetValue("action").ToString();
                    switch (a)
                    {
                        case "flowrequest":
                            await InvokeFlowRequest(connectionId, message);
                            break;
                    }
                }

            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public async Task SendMessage(string connectionId, string message)
        {
            if (dicWebSockets.ContainsKey(connectionId))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await dicWebSockets[connectionId].SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private string CreateWebSocketId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private async Task InvokeFlowRequest(string connectionId, JObject message)
        {
            JObject requestData = new JObject();
            requestData.Add("processId", message.GetValue("data"));
            requestData.Add("connectionId", connectionId);

            using (var req = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:6924{ProjectVariableConstants.BpmsUrlPath}/flow/request")
            {
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Bearer", message.GetValue("token").ToString())
                },
                Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
            })
            {
                using (var response = await _httpClient.SendAsync(req))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(responseContent);
                }
            }
        }
    }
}
