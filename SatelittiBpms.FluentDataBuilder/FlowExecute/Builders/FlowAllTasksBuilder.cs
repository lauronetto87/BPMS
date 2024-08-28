using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Builders
{
    public class FlowAllTasksBuilder : BaseBuilderFlow
    {
        private readonly FlowBuilder _parent;

        public FlowAllTasksBuilder(FlowBuilder parent)
        {
            _parent = parent;
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
        public FlowAllTasksBuilder FieldValue(DataId fieldId, object value = null)
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
