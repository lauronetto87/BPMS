using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System.Collections.Generic;
using System.Xml;

namespace SatelittiBpms.Workflow.Services
{
    public class ExclusiveGatewayParseService : IExclusiveGatewayService
    {
        private readonly IXmlDiagramService _xmlDiagramService;

        public ExclusiveGatewayParseService(
            IXmlDiagramService xmlDiagramService)
        {
            _xmlDiagramService = xmlDiagramService;
        }
        public Dictionary<string, object> AddBranchExclusiveGatewayActivity(XmlNode processNode, XmlNode nodeToProcess, List<ActivityUserOptionInfo> activityOptions)
        {
            var dicBranch = new Dictionary<string, object>();
            var lstNodeOutgoing = _xmlDiagramService.ListNodeOutgoing(processNode, nodeToProcess);
            var incomingValue = _xmlDiagramService.SelectIncomingValue(nodeToProcess);
            var incomingNodeId = _xmlDiagramService.SelectNodeWithOutgoing(processNode, incomingValue)?.Attributes["id"].Value;

            foreach (XmlNode nodeOutgoing in lstNodeOutgoing)
            {
                var valueOption = _xmlDiagramService.SelectSequenceFlow(nodeOutgoing, nodeOutgoing.InnerText)?.Attributes["satelitti:option"].Value;
                dicBranch.Add(
                    _xmlDiagramService.SelectSingleNodeWithIncomingText(processNode, nodeOutgoing.InnerText)?.Attributes["id"].Value,
                    $"data.Option == \"{activityOptions.Find(x => x.Description == valueOption && x.ActivityUser.Activity.ComponentInternalId == incomingNodeId).Id}\""
                );
            }

            return dicBranch;
        }
    }
}
