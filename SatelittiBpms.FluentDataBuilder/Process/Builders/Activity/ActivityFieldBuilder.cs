using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity
{
    public class ActivityFieldBuilder : FieldBaseBuilder
    {
        public ActivityFieldBuilder(ContextBuilder context, ActivityBaseBuilder parent, DataId fieldId) : base(context, parent, fieldId)
        {
            Parent = parent;
        }

        public ExclusiveGatewayBuilder ExclusiveGateway(DataId id = null)
        {
            return FindFirstParentOrThis<IActivityParent>().ExclusiveGateway(id);
        }


        internal ActivityBaseBuilder Parent { get; private set; }

        public ActivityFieldBuilder Field(DataId fieldId = null)
        {
            return FindFirstParentOrThis<ActivityUserBuilder>().Field(fieldId);
        }

        public ActivityFieldBuilder Field(DataId fieldId = null, Models.Enums.FieldTypeEnum? type = null)
        {   
            var fieldBuilder = FindFirstParentOrThis<ActivityUserBuilder>().Field(fieldId);
            fieldBuilder.Type(type.Value);
            return fieldBuilder;                
        }

        public ActivityFieldBuilder Id(DataId id)
        {
            FieldId = id;
            return this;
        }


        public ActivityFieldBuilder Type(Models.Enums.FieldTypeEnum type)
        {
            _type = type;
            return this;
        }

        private Models.Enums.ProcessTaskFieldStateEnum? _state;
        public ActivityFieldBuilder State(Models.Enums.ProcessTaskFieldStateEnum state)
        {
            _state = state;
            return this;
        }

        internal new ActivityFieldData LastBuild => base.LastBuild as ActivityFieldData;
        internal override IData Build()
        {
            return new ActivityFieldData()
            {
                Id = FieldId ?? new DataId(),
                Label = faker.Name.FullName(),
                Type = _type ?? faker.Random.Enum<Models.Enums.FieldTypeEnum>(),
                State = _state ?? faker.Random.Enum<Models.Enums.ProcessTaskFieldStateEnum>(),
            };
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            return new List<IBaseBuilder>();
        }
    }
}
