using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Tests
{
    public class ActivityUserServiceTest
    {
        Mock<IActivityService> _mockActivityService;
        Mock<IActivityUserOptionService> _mockActivityUserOptionService;
        Mock<IXmlDiagramService> _mockXmlDiagramService;
        Mock<IActivityUserRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        IXmlDiagramService _xmlDiagramService;

        [SetUp]
        public void Setup()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockActivityUserOptionService = new Mock<IActivityUserOptionService>();
            _mockXmlDiagramService = new Mock<IXmlDiagramService>();
            _mockRepository = new Mock<IActivityUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _xmlDiagramService = new XmlDiagramService();
        }

        [Test]
        public async Task ensureThatInsertByDiagramSuccefullyWhenHaveOneActivityUserAndActivityUserOption()
        {
            var diagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_030rvvn</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0yrjryt\" name=\"Atividade A\" satelitti:executorType=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"1\" /><satelitti:taskOption description=\"2\" /><satelitti:taskOption description=\"3\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_030rvvn</bpmn2:incoming><bpmn2:outgoing>Flow_19vfn63</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_030rvvn\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_0yrjryt\" /><bpmn2:endEvent id=\"Event_1qmbxq1\"><bpmn2:incoming>Flow_19vfn63</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_19vfn63\" sourceRef=\"Activity_0yrjryt\" targetRef=\"Event_1qmbxq1\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Process_1\"><bpmndi:BPMNEdge id=\"Flow_030rvvn_di\" bpmnElement=\"Flow_030rvvn\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_19vfn63_di\" bpmnElement=\"Flow_19vfn63\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"652\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0yrjryt_di\" bpmnElement=\"Activity_0yrjryt\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_1qmbxq1_di\" bpmnElement=\"Event_1qmbxq1\"><dc:Bounds x=\"652\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>";
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(diagramContent);
            var processNode = bpmnXml.GetElementsByTagName("bpmn2:process")[0];

            foreach (XmlNode activityNode in processNode.ChildNodes.OfType<XmlElement>())
            {
                if (activityNode.Name == "bpmn2:userTask")
                {
                    var taskOption = _xmlDiagramService.ListOptionNodes(activityNode);

                    _mockXmlDiagramService.Setup(x => x.GetAttributeValue(It.IsAny<XmlNode>(), "id")).Returns(It.IsAny<string>());
                    _mockXmlDiagramService.Setup(x => x.GetUserTaskExecutorType(It.IsAny<XmlNode>())).Returns(Models.Enums.UserTaskExecutorTypeEnum.REQUESTER);
                    _mockXmlDiagramService.Setup(x => x.ListOptionNodes(activityNode)).Returns(taskOption);
                    _mockActivityService.Setup(x => x.GetId(It.IsAny<string>(), 1, 55)).Returns(1);
                    _mockRepository.Setup(x => x.Insert(It.IsAny<ActivityUserInfo>())).ReturnsAsync(3);

                    ActivityUserService activityUserService = new ActivityUserService(_mockActivityUserOptionService.Object, _mockXmlDiagramService.Object, _mockRepository.Object, _mockMapper.Object);
                    await activityUserService.InsertByDiagram(activityNode, 1, 1, 55);

                    _mockXmlDiagramService.Verify(x => x.GetUserTaskExecutorType(It.IsAny<XmlNode>()), Times.Once());
                    _mockRepository.Verify(x => x.Insert(It.IsAny<ActivityUserInfo>()), Times.Once());
                    _mockXmlDiagramService.Verify(x => x.ListOptionNodes(It.IsAny<XmlNode>()), Times.Once());
                    _mockActivityUserOptionService.Verify(x => x.Insert(It.IsAny<ActivityUserOptionDTO>()), Times.Exactly(3));
                }

            }
        }
    }
}
