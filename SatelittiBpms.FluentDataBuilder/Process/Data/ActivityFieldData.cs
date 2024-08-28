using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivityFieldData : FieldBaseData
    {
        public ProcessTaskFieldStateEnum State { get; set; } = ProcessTaskFieldStateEnum.EDITABLE;
    }
}
