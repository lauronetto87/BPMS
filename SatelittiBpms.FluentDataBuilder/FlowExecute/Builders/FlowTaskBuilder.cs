using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Builders
{
    public class FlowTaskBuilder : BaseBuilderFlow
    {
        private readonly FlowBuilder _parent;

        public FlowTaskBuilder(FlowBuilder parent)
        {
            _parent = parent;
        }

        public FlowTaskBuilder ExecuteTask()
        {
            return _parent.ExecuteTask();
        }

        public FlowBuilder NewFlow()
        {
            return _parent.NewFlow();
        }

        public override FlowCollectionData EndCreateFlows()
        {
            return _parent.EndCreateFlows();
        }

        internal readonly List<FlowFieldValue> _flowFieldValue = new();
        public FlowTaskBuilder FieldValue(DataId fieldId, object value = null)
        {
            var data = new FlowFieldValue(fieldId, value);
            _flowFieldValue.Add(data);
            return this;
        }

        internal FlowTaskData Build()
        {
            return new FlowTaskData
            {
                FieldValues = _flowFieldValue,
            };
        }
    }
}
