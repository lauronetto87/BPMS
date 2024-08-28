using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public abstract class ActivityBaseData : BaseData
    {
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public WorkflowActivityTypeEnum ActivityType { get; set; }
    }
}
