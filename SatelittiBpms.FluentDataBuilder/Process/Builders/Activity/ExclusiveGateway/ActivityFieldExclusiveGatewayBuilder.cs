namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway
{
    public class ActivityFieldExclusiveGatewayBuilder : ActivityFieldBuilder
    {
        public ActivityFieldExclusiveGatewayBuilder(ContextBuilder context, ActivityBaseBuilder parent, DataId fieldId) : base(context, parent, fieldId)
        {
        }

        public ExclusiveGatewayBranchBuilder Branch()
        {
            return FindFirstParentOrThis<ExclusiveGatewayBuilder>().Branch();
        }

        public new ActivityUserExclusiveGatewayBuilder ActivityUser(DataId id = null)
        {
            return (ActivityUserExclusiveGatewayBuilder)base.ActivityUser(id);
        }
        public new ActivitySendExclusiveGatewayBuilder ActivitySend(DataId id = null)
        {
            return (ActivitySendExclusiveGatewayBuilder)base.ActivitySend(id);
        }


        public new ActivityFieldExclusiveGatewayBuilder Field(DataId fieldId = null)
        {
            return FindFirstParentOrThis<ActivityUserExclusiveGatewayBuilder>().Field(fieldId);
        }
        public new ActivityFieldExclusiveGatewayBuilder Id(DataId id)
        {
            FieldId = id;
            return this;
        }
        public new ActivityFieldExclusiveGatewayBuilder Type(Models.Enums.FieldTypeEnum type)
        {
            return (ActivityFieldExclusiveGatewayBuilder)base.Type(type);
        }

        public new ActivityFieldExclusiveGatewayBuilder State(Models.Enums.ProcessTaskFieldStateEnum state)
        {
            return (ActivityFieldExclusiveGatewayBuilder)base.State(state);
        }
    }
}
