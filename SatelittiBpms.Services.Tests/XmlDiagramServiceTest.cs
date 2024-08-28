using NUnit.Framework;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Enums;
using System.Xml;

namespace SatelittiBpms.Services.Tests
{
    public class XmlDiagramServiceTest
    {
        private XmlNamespaceManager nsmgr;

        [SetUp]
        public void Setup()
        {
            NameTable nameTable = new NameTable();
            nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace(XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX, XmlDiagramConstants.BPMN2_NAMESPACE_URI);
            nsmgr.AddNamespace(XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX, XmlDiagramConstants.SATELITTI_NAMESPACE_URI);
        }

        [Test]
        public void ensureThatGetAttributeValueReturnsWhenHaveAttribute()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_0z4exk4']", nsmgr);
            var result = xmlDiagramService.GetAttributeValue(node, "name");
            Assert.AreEqual("Atv2", result);
            result = xmlDiagramService.GetAttributeValue(node, "satelitti:executorType");
            Assert.AreEqual("2", result);
            result = xmlDiagramService.GetAttributeValue(node, "satelitti:executorId");
            Assert.AreEqual("1", result);
        }

        [Test]
        public void ensureThatGetAttributeValueReturnsNullWhenNoHaveAttribute()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_0z4exk4']", nsmgr);
            var result = xmlDiagramService.GetAttributeValue(node, "named");
            Assert.IsNull(result);
            result = xmlDiagramService.GetAttributeValue(node, "satelitti:executorTypetgt");
            Assert.IsNull(result);
            result = xmlDiagramService.GetAttributeValue(node, "satelitti:executorIdsdsd");
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatGetUserTaskExecutorTypeReturnsRequester()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_14tq3bf']", nsmgr);
            var result = xmlDiagramService.GetUserTaskExecutorType(node);
            Assert.AreEqual(UserTaskExecutorTypeEnum.REQUESTER, result);
        }

        [Test]
        public void ensureThatGetUserTaskExecutorTypeReturnsRole()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_0z4exk4']", nsmgr);
            var result = xmlDiagramService.GetUserTaskExecutorType(node);
            Assert.AreEqual(UserTaskExecutorTypeEnum.ROLE, result);
        }

        [Test]
        public void ensureThatGetNodeOptionsReturnsASingleOption()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_14tq3bf']", nsmgr);

            var result = xmlDiagramService.ListOptionNodes(node);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Solicitar", xmlDiagramService.GetAttributeValue(result[0], "description"));
        }

        [Test]
        public void ensureThatGetNodeOptionsReturnsManyOptions()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_0z4exk4']", nsmgr);

            var result = xmlDiagramService.ListOptionNodes(node);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Aprovar", xmlDiagramService.GetAttributeValue(result[0], "description"));
            Assert.AreEqual("Reprovar", xmlDiagramService.GetAttributeValue(result[1], "description"));
        }

        [Test]
        public void ensureThatGetExecutorIdAttributeValueReturnsWhenAttribute()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_0z4exk4']", nsmgr);

            var result = xmlDiagramService.GetExecutorIdAttributeValue(node);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ensureThatGetNextStepNodeReturnNullWhenNotHaveOutgoing()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:endEvent[@id='Event_0su4wlb']", nsmgr);

            var result = xmlDiagramService.GetNextStepNode(bpmnXml.GetElementsByTagName("bpmn2:process")[0], node);
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatGetNextStepNodeReturnNullWhenNotHaveIncomingWithOutgoingValue()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:userTask[@id='Activity_14tdsff']", nsmgr);

            var result = xmlDiagramService.GetNextStepNode(bpmnXml.GetElementsByTagName("bpmn2:process")[0], node);
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatGetNextStepNodeReturnNextStepNodeWhenHaveIncomingWithOutgoinValue()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_0uuck5l']", nsmgr);

            var result = xmlDiagramService.GetNextStepNode(bpmnXml.GetElementsByTagName("bpmn2:process")[0], node);
            Assert.IsNotNull(result);
            Assert.AreEqual("Event_0su4wlb", xmlDiagramService.GetAttributeValue(result, "id"));
            Assert.AreEqual("Fim", xmlDiagramService.GetAttributeValue(result, "name"));
        }

        [Test]
        public void ensureThatGetSendTaskDestinataryTypeReturnsRequester()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_0wpxjeg']", nsmgr);
            var result = xmlDiagramService.GetSendTaskDestinataryType(node);
            Assert.AreEqual(SendTaskDestinataryTypeEnum.REQUESTER, result);
        }

        [Test]
        public void ensureThatGetDestinataryIdAttributeValueReturnsWhenAttribute()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_0uuck5l']", nsmgr);

            var result = xmlDiagramService.GetDestinataryIdAttributeValue(node);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ensureThatGetMessageNotificationReturnsMessage()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_0uuck5l']", nsmgr);

            var result = xmlDiagramService.GetMessageNotification(node);
            Assert.AreEqual("Email teste aprovador", result);
        }

        [Test]
        public void ensureThatGetTitleMessageNotificationReturnsTitle()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_0uuck5l']", nsmgr);

            var result = xmlDiagramService.GetTitleMessageNotification(node);
            Assert.AreEqual("Email2", result);
        }

        [Test]
        public void ensureListNodeOutgoingReturnNullWhenNotHaveOutgoing()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:endEvent[@id='Event_0su4wlb']", nsmgr);

            var result = xmlDiagramService.ListNodeOutgoing(bpmnXml.GetElementsByTagName("bpmn2:process")[0], node);
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatListNodeOutgoingReturnNextStepNodeWhenHaveIncomingWithOutgoinValue()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_diagramWithGateway));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:exclusiveGateway[@id='Gateway_1dd9usy']", nsmgr);

            var result = xmlDiagramService.ListNodeOutgoing(bpmnXml.GetElementsByTagName("bpmn2:process")[0], node);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Flow_083ti31", result[0].InnerText);
            Assert.AreEqual("Flow_15x3v1t", result[1].InnerText);
        }

        [Test]
        public void ensureThatSelectSingleNodeWithIncomingTextReturnsNullWhenNotHaveIncoming()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var result = xmlDiagramService.SelectSingleNodeWithIncomingText(bpmnXml.GetElementsByTagName("bpmn2:process")[0], "abasa");
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatSelectSingleNodeWithIncomingTextReturnsWhenHaveIncoming()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var result = xmlDiagramService.SelectSingleNodeWithIncomingText(bpmnXml.GetElementsByTagName("bpmn2:process")[0], "Flow_122rusw");
            Assert.IsNotNull(result);
            Assert.AreEqual("Activity_0wpxjeg", xmlDiagramService.GetAttributeValue(result, "id"));
            Assert.AreEqual("Atv3", xmlDiagramService.GetAttributeValue(result, "name"));
        }

        [Test]
        public void ensureThatSelectSequenceFlowReturnsNullWhenNotHaveSequenceFlow()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var result = xmlDiagramService.SelectSequenceFlow(bpmnXml.GetElementsByTagName("bpmn2:process")[0], "dsdsds");
            Assert.IsNull(result);
        }

        [Test]
        public void ensureThatSelectSequenceFlowReturnsWhenHaveSequenceFlow()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.xmlServiceTest_compleDiagram));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var result = xmlDiagramService.SelectSequenceFlow(bpmnXml.GetElementsByTagName("bpmn2:process")[0], "Flow_08sniww");
            Assert.IsNotNull(result);
            Assert.AreEqual("Activity_14tq3bf", xmlDiagramService.GetAttributeValue(result, "sourceRef"));
            Assert.AreEqual("Activity_0z4exk4", xmlDiagramService.GetAttributeValue(result, "targetRef"));
        }


        [Test]
        public void ensureThatGetCustomEmailAttributeValueReturnsWhenAttribute()
        {
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_activitySendEmailWithCustomEmail));

            XmlDiagramService xmlDiagramService = new XmlDiagramService();
            var node = bpmnXml.SelectSingleNode("//bpmn2:sendTask[@id='Activity_1fotvle']", nsmgr);

            var result = xmlDiagramService.GetCustomEmailAttributeValue(node);
            Assert.AreEqual("erli.soares@selbetti.com.br", result);
        }

    }
}
