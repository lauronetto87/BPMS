using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using SatelittiBpms.Workflow.Interfaces;
using SatelittiBpms.Workflow.Services;
using SatelittiBpms.Workflow.WorkflowCoreElements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Workflow.Tests.Services
{
    public class ExclusiveGatewayServiceTest
    {
        Mock<IXmlDiagramService> _mockXmlDiagramService;
        Mock<IActivityService> _mockActivityService;
        
        Mock<IActivityUserOptionService> _mockActivityUserOptionService;

        IXmlDiagramService _xmlDiagramService;
        IExclusiveGatewayService _exclusiveGatewayService;

        [SetUp]
        public void Setup()
        {
            _mockXmlDiagramService = new Mock<IXmlDiagramService>();
            _mockActivityService = new Mock<IActivityService>();
            _mockActivityUserOptionService = new Mock<IActivityUserOptionService>();

            _xmlDiagramService = new XmlDiagramService();
            _exclusiveGatewayService = new ExclusiveGatewayParseService(_xmlDiagramService);
        }

        [Test]
        public async Task ensureThatInsertBranchExclusiveGateway()
        {
            #region SETUP INFORMACOES

            int processId = 1,
                version = 2,
                processVersionId = 2,
                tenantId = 55;

            ActivityUserOptionInfo option1 = new ActivityUserOptionInfo()
            {
                Id = 5,
                Description = "Aprovado",
                TenantId = 55,
                ActivityUserId = 17,
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Activity_0z5xvro",
                        Name = "Atividade 1"
                    }
                }
            };
            ActivityUserOptionInfo option2 = new ActivityUserOptionInfo()
            {
                Id = 6,
                Description = " Reprovado",
                TenantId = 55,
                ActivityUserId = 17,
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Activity_0z5xvro",
                        Name = "Atividade 1"
                    }
                }
            };
            ActivityUserOptionInfo option3 = new ActivityUserOptionInfo()
            {
                Id = 7,
                Description = "Finalizar",
                TenantId = 55,
                ActivityUserId = 19,
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Activity_09m4anl",
                        Name = "Atividade Aprovada"
                    }
                }
            };
            ActivityUserOptionInfo option4 = new ActivityUserOptionInfo()
            {
                Id = 8,
                Description = " Finalizar",
                TenantId = 55,
                ActivityUserId = 20,
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Activity_1b779lf",
                        Name = "Atividade Reprovada"
                    }
                }
            };

            List<ActivityUserOptionInfo> activityUserOptions = new List<ActivityUserOptionInfo>();
            activityUserOptions.Add(option1);
            activityUserOptions.Add(option2);
            activityUserOptions.Add(option3);
            activityUserOptions.Add(option4);

            List<ActivityInfo> activities = new List<ActivityInfo>();
            ActivityInfo activity1 = new ActivityInfo()
            {
                ComponentInternalId = "StartEvent_1",
                Id = 16,
                ProcessVersionId = 2,
                TenantId = 55
            };
            ActivityInfo activity2 = new ActivityInfo()
            {
                ComponentInternalId = "Activity_0z5xvro",
                Id = 17,
                Name = "Atividade 1",
                ProcessVersionId = 2,
                TenantId = 55,
                ActivityUser = new ActivityUserInfo()
                {
                    RoleId = 4,
                    TenantId = 55,
                    ExecutorType = SatelittiBpms.Models.Enums.UserTaskExecutorTypeEnum.ROLE,
                    ActivityUsersOptions = new List<ActivityUserOptionInfo>()
                    {
                        
                    }
                },
                
            };
            ActivityInfo activity3 = new ActivityInfo()
            {
                ComponentInternalId = "Gateway_1x04tq8",
                Name = "Análise",
                Id = 18,
                ProcessVersionId = 2,
                TenantId = 55
            };
            ActivityInfo activity4 = new ActivityInfo()
            {
                ComponentInternalId = "Activity_09m4anl",
                Name = "Atividade Aprovada",
                Id = 19,
                ProcessVersionId = 2,
                TenantId = 55,
                ActivityUser = new ActivityUserInfo()
                {
                    RoleId = 4,
                    TenantId = 55,
                    ExecutorType = SatelittiBpms.Models.Enums.UserTaskExecutorTypeEnum.ROLE
                }
            };
            ActivityInfo activity5 = new ActivityInfo()
            {
                ComponentInternalId = "Activity_1b779lf",
                Name = "Atividade Reprovada",
                Id = 20,
                ProcessVersionId = 2,
                TenantId = 55,
                ActivityUser = new ActivityUserInfo()
                {
                    TenantId = 55,
                    ExecutorType = SatelittiBpms.Models.Enums.UserTaskExecutorTypeEnum.REQUESTER
                }
            };
            ActivityInfo activity6 = new ActivityInfo()
            {
                ComponentInternalId = "Event_0in9lb7",
                Id = 21,
                ProcessVersionId = 2,
                TenantId = 55,
                
            };
            activities.Add(activity1);
            activities.Add(activity2);
            activities.Add(activity3);
            activities.Add(activity4);
            activities.Add(activity5);
            activities.Add(activity6);
            #endregion


            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_ProcessWithSimpleDecision));
            var processNode = bpmnXml.GetElementsByTagName("bpmn2:process")[0];

            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(activities);
            _mockActivityUserOptionService.Setup(x => x.ListByUserActivityId(It.IsAny<List<int>>())).ReturnsAsync(activityUserOptions);


            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _exclusiveGatewayService, _mockActivityUserOptionService.Object);
            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);
            WorkflowElement stepElementExclusive = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            Assert.AreEqual(processId.ToString(), stepElementExclusive.Id);
            Assert.AreEqual(version, stepElementExclusive.Version);
            var exclusive = (StepElementExclusive)stepElementExclusive.Steps[2];

            Assert.AreEqual("Gateway_1x04tq8", exclusive.Id);
            Assert.AreEqual(ExclusiveGatewayActivity.TypeDescription, exclusive.StepType);
            Assert.AreEqual(2, exclusive.SelectNextStep.Count());
            Assert.AreEqual("data.Option == \"5\"", exclusive.SelectNextStep["Activity_09m4anl"]);
            Assert.AreEqual("data.Option == \"6\"", exclusive.SelectNextStep["Activity_1b779lf"]);
        }
    }
}
