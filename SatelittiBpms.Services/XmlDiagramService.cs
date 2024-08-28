using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Xml;

namespace SatelittiBpms.Services
{
    public class XmlDiagramService : IXmlDiagramService
    {
        private readonly XmlNamespaceManager nsmgr;

        public XmlDiagramService()
        {
            var nameTable = new NameTable();
            nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace(XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX, XmlDiagramConstants.BPMN2_NAMESPACE_URI);
            nsmgr.AddNamespace(XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX, XmlDiagramConstants.SATELITTI_NAMESPACE_URI);
        }

        public string GetAttributeValue(XmlNode xmlNode, string tagAttribute)
        {
            return xmlNode.Attributes[tagAttribute]?.Value;
        }

        public UserTaskExecutorTypeEnum GetUserTaskExecutorType(XmlNode xmlNode)
        {
            var executorType = GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXECUTOR_TYPE_ATTRIBUTE}");
            return (UserTaskExecutorTypeEnum)Convert.ToInt32(executorType);
        }

        public XmlNodeList ListOptionNodes(XmlNode node)
        {
            return node.SelectNodes($".//{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.TASK_OPTION_NODE_NAME}", nsmgr);
        }

        public int GetExecutorIdAttributeValue(XmlNode xmlNode)
        {
            var executorId = GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXECUTOR_ID_ATTRIBUTE}");
            return Convert.ToInt32(executorId);
        }

        public int GetPersonIdAttributeValue(XmlNode xmlNode)
        {
            var executorId = GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.PERSON_ID_ATTRIBUTE}");
            return Convert.ToInt32(executorId);
        }

        public XmlNode GetNextStepNode(XmlNode processNode, XmlNode nodeToProcess)
        {
            XmlNode outgoingNode = nodeToProcess.SelectSingleNode($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNode == null)
                return null;

            return SelectSingleNodeWithIncomingText(processNode, outgoingNode.InnerText);
        }

        public SendTaskDestinataryTypeEnum GetSendTaskDestinataryType(XmlNode xmlNode)
        {
            var sendDestinatary = GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.DESTINATARY_TYPE_ATTRIBUTE}");
            return (SendTaskDestinataryTypeEnum)Convert.ToInt32(sendDestinatary);
        }

        public int? GetDestinataryIdAttributeValue(XmlNode xmlNode)
        {
            var destinataryId = GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.DESTINATARY_ID_ATTRIBUTE}");
            return Convert.ToInt32(destinataryId);
        }

        public string GetCustomEmailAttributeValue(XmlNode xmlNode)
        {
            return GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.CUSTOM_EMAIL_ID_ATTRIBUTE}");
        }

        public string GetMessageNotification(XmlNode xmlNode)
        {
            return GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.MESSAGE_NOTIFICATION}");
        }

        public string GetTitleMessageNotification(XmlNode xmlNode)
        {
            return GetAttributeValue(xmlNode, $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.TITLE_MESSAGE_NOTIFICATION}");
        }

        public XmlNodeList ListNodeOutgoing(XmlNode processNode, XmlNode nodeToProcess)
        {
            XmlNodeList outgoingNode = nodeToProcess.SelectNodes($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNode.Count <= 0)
                return null;

            return outgoingNode;
        }

        public XmlNode SelectSingleNodeWithIncomingText(XmlNode processNode, string innerText)
        {
            XmlNode singleNodeIncoming = processNode.SelectSingleNode($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}[text()='{innerText}']", nsmgr);
            return singleNodeIncoming?.ParentNode;
        }

        public XmlNode SelectSequenceFlow(XmlNode processNode, string id)
        {
            return processNode.SelectSingleNode($"//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:sequenceFlow[@id='{id}']", nsmgr);
        }

        public string SelectIncomingValue(XmlNode nodeToProcess)
        {
            XmlNode singleNodeIncoming = nodeToProcess.SelectSingleNode($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr);
            return singleNodeIncoming?.InnerText;
        }

        public XmlNode SelectNodeWithOutgoing(XmlNode processNode, string outgoingId)
        {
            XmlNode singleOutgoingNote = processNode.SelectSingleNode($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}[text()='{outgoingId}']", nsmgr);
            return singleOutgoingNote?.ParentNode;
        }

    }
}
