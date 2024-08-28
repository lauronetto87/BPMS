using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.VersionNormalization.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.VersionNormalization.Normalizations
{
    public class _20211206_WebSocketWorkFlowContentNormalization : IVersionNormalization
    {
        IProcessVersionService _processVersionService;

        public _20211206_WebSocketWorkFlowContentNormalization(IServiceProvider services)
        {
            _processVersionService = services.GetRequiredService<IProcessVersionService>();
        }

        public async Task Execute()
        {
            var processVersionList = await _processVersionService.ListAsync();

            foreach (ProcessVersionInfo processVersion in processVersionList.Where(x => x.WorkflowContent != null))
            {
                var parent = JsonConvert.DeserializeObject<JObject>(processVersion.WorkflowContent, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

                ((JArray)parent.Property("Steps").Value)
                .Select(jo => (JObject)jo)
                .ToList()
                .ForEach(x =>
                {
                    if (x.Property("StepType").Value.ToString().Contains("SatelittiBpms.Workflow.ActivityTypes.StartEventActivity"))
                    {
                        ((JObject)x.Property("Inputs").Value)?.Property("FlowId")?.Remove();
                        ((JObject)x.Property("Inputs").Value)?.Property("TaskId")?.Remove();
                    }

                    if (x.Property("StepType").Value.ToString().Contains("SatelittiBpms.Workflow.ActivityTypes.UserTaskActivity"))
                    {
                        ((JObject)x.Property("Inputs").Value)?.Property("ProcessVersionId")?.Remove();

                        if (((JObject)x.Property("Inputs").Value)?.Property("ConnectionId") == null)
                        {
                            ((JObject)x.Property("Inputs").Value)?.Add("ConnectionId", "data.ConnectionId");
                        }
                    }

                    if (x.Property("StepType").Value.ToString().Contains("SatelittiBpms.Workflow.ActivityTypes.SendTaskActivity"))
                    {
                        ((JObject)x.Property("Inputs").Value)?.Property("ProcessVersionId")?.Remove();

                        if (((JObject)x.Property("Inputs").Value)?.Property("ConnectionId") == null)
                        {
                            ((JObject)x.Property("Inputs").Value)?.Add("ConnectionId", "data.ConnectionId");
                        }
                    }

                    if (x.Property("StepType").Value.ToString().Contains("SatelittiBpms.Workflow.ActivityTypes.ExclusiveGatewayActivity"))
                    {
                        ((JObject)x.Property("Inputs").Value)?.Property("ProcessVersionId")?.Remove();
                        ((JObject)x.Property("Inputs").Value)?.Property("RequesterId")?.Remove();
                    }

                    if (x.Property("StepType").Value.ToString().Contains("SatelittiBpms.Workflow.ActivityTypes.EndEventActivity"))
                    {
                        ((JObject)x.Property("Inputs").Value)?.Property("RequesterId")?.Remove();
                        ((JObject)x.Property("Inputs").Value)?.Property("ProcessVersionId")?.Remove();

                        if (((JObject)x.Property("Inputs").Value)?.Property("ConnectionId") == null)
                        {
                            ((JObject)x.Property("Inputs").Value)?.Add("ConnectionId", "data.ConnectionId");
                        }
                    }

                });

                string workflowContent = JsonConvert.SerializeObject(parent, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

                await _processVersionService.UpdateWorkFlowContent(processVersion.Id, workflowContent);
            }
        }
    }
}
