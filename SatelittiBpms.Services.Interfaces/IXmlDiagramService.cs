using SatelittiBpms.Models.Enums;
using System.Xml;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IXmlDiagramService
    {
        string GetAttributeValue(XmlNode xmlNode, string tagAttribute);
        UserTaskExecutorTypeEnum GetUserTaskExecutorType(XmlNode xmlNode);
        int GetExecutorIdAttributeValue(XmlNode xmlNode);
        int GetPersonIdAttributeValue(XmlNode xmlNode);
        XmlNodeList ListOptionNodes(XmlNode node);
        XmlNode GetNextStepNode(XmlNode processNode, XmlNode nodeToProcess);
        XmlNodeList ListNodeOutgoing(XmlNode processNode, XmlNode nodeToProcess);
        XmlNode SelectSingleNodeWithIncomingText(XmlNode processNode, string innerText);
        XmlNode SelectSequenceFlow(XmlNode processNode, string innerText);
        SendTaskDestinataryTypeEnum GetSendTaskDestinataryType(XmlNode xmlNode);
        int? GetDestinataryIdAttributeValue(XmlNode xmlNode);
        string GetCustomEmailAttributeValue(XmlNode xmlNode);
        string GetMessageNotification(XmlNode xmlNode);
        string GetTitleMessageNotification(XmlNode xmlNode);
        XmlNode SelectNodeWithOutgoing(XmlNode processNode, string outgoingId);
        string SelectIncomingValue(XmlNode nodeToProcess);
    }
}
