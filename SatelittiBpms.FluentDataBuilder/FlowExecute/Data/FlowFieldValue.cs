using SatelittiBpms.FluentDataBuilder.Process;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Data
{
    public class FlowFieldValue
    {
        public DataId FieldId { get; internal set; }
        public object Value { get; internal set; }

        public FlowFieldValue(DataId fieldId, object value)
        {
            FieldId = fieldId;
            Value = value;
        }
        
    }
}
