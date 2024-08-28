using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ExclusiveGatewayData : ActivityBaseData
    {
        public IList<ExclusiveGatewayBranchData> Branchs { get; set; }
    }
}
