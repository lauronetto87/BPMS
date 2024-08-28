using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public abstract class FieldBaseData : BaseData
    {
        public DataId Id { get; set; }
        public string Label { get; set; }
        public FieldTypeEnum Type { get; set; }
    }
}
