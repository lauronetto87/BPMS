using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Tests
{
    public class ActivityNotificationServiceTest
    {
        Mock<IActivityService> _mockActivityService;
        Mock<IXmlDiagramService> _mockXmlDiagramService;
        Mock<IActivityNotificationRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        IXmlDiagramService _xmlDiagramService;

        [SetUp]
        public void Setup()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockXmlDiagramService = new Mock<IXmlDiagramService>();
            _mockRepository = new Mock<IActivityNotificationRepository>();
            _mockMapper = new Mock<IMapper>();
            _xmlDiagramService = new XmlDiagramService();
        }

        [Test]
        public async Task ensureThatInsertByDiagramSuccefullyWhenHaveOneActivityNotification()
        {
            var diagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\" name=\"Inicio\"><bpmn2:outgoing>Flow_1astrbe</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0szk0t0\" name=\"Pedido de Ferias\" satelitti:executorType=\"2\" satelitti:executorId=\"1\"><bpmn2:extensionElements><satelitti:taskOptions><satelitti:taskOption description=\"Confirmar\" /></satelitti:taskOptions></bpmn2:extensionElements><bpmn2:incoming>Flow_1astrbe</bpmn2:incoming><bpmn2:outgoing>Flow_07dqka7</bpmn2:outgoing></bpmn2:userTask><bpmn2:sendTask id=\"Activity_0uqyffu\" name=\"Notificação de Ferias\" satelitti:message=\"Notificação de Ferias\" satelitti:destinataryType=\"1\" satelitti:titleMessage=\"Notificação de Férias\"><bpmn2:incoming>Flow_07dqka7</bpmn2:incoming><bpmn2:outgoing>Flow_1u0aold</bpmn2:outgoing></bpmn2:sendTask><bpmn2:endEvent id=\"Event_0t9pcob\" name=\"Fim\"><bpmn2:incoming>Flow_1u0aold</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_1astrbe\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_0szk0t0\" /><bpmn2:sequenceFlow id=\"Flow_07dqka7\" sourceRef=\"Activity_0szk0t0\" targetRef=\"Activity_0uqyffu\" /><bpmn2:sequenceFlow id=\"Flow_1u0aold\" sourceRef=\"Activity_0uqyffu\" targetRef=\"Event_0t9pcob\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Process_1\"><bpmndi:BPMNEdge id=\"Flow_1astrbe_di\" bpmnElement=\"Flow_1astrbe\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"630\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_07dqka7_di\" bpmnElement=\"Flow_07dqka7\"><di:waypoint x=\"730\" y=\"258\" /><di:waypoint x=\"890\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_1u0aold_di\" bpmnElement=\"Flow_1u0aold\"><di:waypoint x=\"990\" y=\"258\" /><di:waypoint x=\"1202\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"417\" y=\"283\" width=\"26\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0szk0t0_di\" bpmnElement=\"Activity_0szk0t0\"><dc:Bounds x=\"630\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0uqyffu_di\" bpmnElement=\"Activity_0uqyffu\"><dc:Bounds x=\"890\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_0t9pcob_di\" bpmnElement=\"Event_0t9pcob\"><dc:Bounds x=\"1202\" y=\"240\" width=\"36\" height=\"36\" /><bpmndi:BPMNLabel><dc:Bounds x=\"1211\" y=\"283\" width=\"19\" height=\"14\" /></bpmndi:BPMNLabel></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>";
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(diagramContent);
            var processNode = bpmnXml.GetElementsByTagName("bpmn2:process")[0];

            foreach (XmlNode activityNode in processNode.ChildNodes.OfType<XmlElement>())
            {
                if (activityNode.Name == "bpmn2:sendTask")
                {    
                    _mockXmlDiagramService.Setup(x => x.GetAttributeValue(It.IsAny<XmlNode>(), "id")).Returns(It.IsAny<string>());
                    _mockActivityService.Setup(x => x.GetId(It.IsAny<string>(), 1, 55)).Returns(1);
                    _mockRepository.Setup(x => x.Insert(It.IsAny<ActivityNotificationInfo>())).ReturnsAsync(3);

                    ActivityNotificationService activityNotificationService = new ActivityNotificationService( _mockXmlDiagramService.Object, _mockRepository.Object, _mockMapper.Object);
                    await activityNotificationService.InsertByDiagram(activityNode, 1, 55);
               
                    _mockRepository.Verify(x => x.Insert(It.IsAny<ActivityNotificationInfo>()), Times.Once());
                }

            }
        }
    }
}
