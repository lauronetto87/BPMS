using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ExclusiveGatewayBranchData : BaseData, IActivityParentData
    {
        public IList<ActivityBaseData> Activities { get; set; }
        public ButtonData Button { get; set; }
    }
}
