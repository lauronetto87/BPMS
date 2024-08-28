using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Xml;

namespace SatelittiBpms.Workflow.Interfaces
{
    public interface IExclusiveGatewayService
    {
        Dictionary<string, object> AddBranchExclusiveGatewayActivity(XmlNode processNode, XmlNode nodeToProcess, List<ActivityUserOptionInfo> activityOptions);
    }
}
