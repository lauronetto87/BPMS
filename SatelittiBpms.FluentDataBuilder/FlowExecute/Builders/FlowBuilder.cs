using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Builders
{
    public class FlowBuilder : BaseBuilderFlow
    {
        private readonly FlowBuilder _parent;
        private readonly Process.Data.ProcessVersionData _processVersionData;
        public FlowBuilder(Process.Data.ProcessVersionData processVersionData, FlowBuilder parent)
        {
            _processVersionData = processVersionData;
            _parent = parent;
        }

        private FlowAllTasksBuilder _flowAllTasksBuilder; 
        public FlowAllTasksBuilder ExecuteAllTasks()
        {
            _flowAllTasksBuilder = new FlowAllTasksBuilder(this);
            return _flowAllTasksBuilder;
        }

        private readonly List<FlowTaskBuilder> _flowTaskBuilders = new ();
        public FlowTaskBuilder ExecuteTask()
        {
            var flowTaskBuilder = new FlowTaskBuilder(this);
            _flowTaskBuilders.Add(flowTaskBuilder);
            return flowTaskBuilder;
        }

        internal readonly List<FlowBuilder> _flowBuilderBuilders = new();
        public FlowBuilder NewFlow()
        {
            if (_parent != null)
            {
                return _parent.NewFlow();
            }
            var flowBuilder = new FlowBuilder(_processVersionData, this);
            _flowBuilderBuilders.Add(flowBuilder);
            return flowBuilder;
        }

        private FlowData MakeFlow()
        {
            if (_flowAllTasksBuilder == null)
            {
                return new FlowData
                {
                    Tasks = _flowTaskBuilders.Select(x => x.Build()).ToList(),
                };
            }


            var flowsAll = new FlowData
            {
                Tasks = new List<FlowTaskData>(),
            };

            foreach (var activity in _processVersionData.AllActivities)
            {
                flowsAll.Tasks.Add(_flowAllTasksBuilder.Build());
            }

            return flowsAll;

            
        }

        public override FlowCollectionData EndCreateFlows()
        {
            if (_parent != null)
            {
                return _parent.EndCreateFlows();
            }

            var flows = new FlowCollectionData
            {
                ProcessVersionData = _processVersionData,
                FlowsData = new List<FlowData>
                {
                    MakeFlow()
                }
            };

            foreach (var flowBuilder in _flowBuilderBuilders)
            {
                flows.FlowsData.Add(flowBuilder.MakeFlow());
            }
            
            return flows;
        }

        internal readonly List<FlowFieldValue> _flowFieldValueBuilders = new();
        public FlowBuilder FieldValue(DataId fieldId, object value = null)
        {
            var builder = new FlowFieldValue(fieldId, value);
            _flowFieldValueBuilders.Add(builder);
            return this;
        }
        
    }
}
