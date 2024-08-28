using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public interface IActivityParentData : IData
    {
        public IList<ActivityBaseData> Activities { get; set; }
    }
}
