namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway
{
    public class ActivityUserExclusiveGatewayBuilder : ActivityUserBuilder
    {
        public ActivityUserExclusiveGatewayBuilder(ContextBuilder context, IBaseBuilder parent, DataId id)
            : base(context, parent, id)
        {
        }

        public ExclusiveGatewayBranchBuilder Branch()
        {
            return FindFirstParentOrThis<ExclusiveGatewayBuilder>().Branch();
        }

        public ActivityUserExclusiveGatewayBuilder ActivityUser(DataId id = null)
        {
            return (ActivityUserExclusiveGatewayBuilder)FindFirstParentOrThis<IActivityParent>().ActivityUser(id);
        }
        public ActivitySendExclusiveGatewayBuilder ActivitySend(DataId id = null)
        {
            return (ActivitySendExclusiveGatewayBuilder)FindFirstParentOrThis<IActivityParent>().ActivitySend(id);
        }

        public new ActivityUserExclusiveGatewayBuilder ExecutorRequester()
        {
            return (ActivityUserExclusiveGatewayBuilder)base.ExecutorRequester();
        }
        public new ActivityUserExclusiveGatewayBuilder ExecutorPerson()
        {
            return (ActivityUserExclusiveGatewayBuilder)base.ExecutorPerson();
        }
        public new ActivityUserExclusiveGatewayBuilder ExecutorRole()
        {
            return (ActivityUserExclusiveGatewayBuilder)base.ExecutorRole();
        }

        public new ActivityUserExclusiveGatewayBuilder Button()
        {
            return (ActivityUserExclusiveGatewayBuilder)base.Button();
        }

        public new ActivityFieldExclusiveGatewayBuilder Field(DataId fieldId = null)
        {
            var builder = new ActivityFieldExclusiveGatewayBuilder(Context, this, fieldId);
            _activityFieldBuilders.Add(builder);
            return builder;
        }
    }
}
