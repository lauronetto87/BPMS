namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway
{
    public class ActivitySendExclusiveGatewayBuilder : ActivitySendBuilder
    {
        public ActivitySendExclusiveGatewayBuilder(ContextBuilder context, IBaseBuilder parent, DataId id) : base(context, parent, id)
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


        public new ActivitySendExclusiveGatewayBuilder DestinataryRole()
        {
            return (ActivitySendExclusiveGatewayBuilder)base.DestinataryRole();
        }
        public new ActivitySendExclusiveGatewayBuilder DestinataryCustom()
        {
            return (ActivitySendExclusiveGatewayBuilder)base.DestinataryCustom();
        }
        public new ActivitySendExclusiveGatewayBuilder DestinataryPerson()
        {
            return (ActivitySendExclusiveGatewayBuilder)base.DestinataryPerson();
        }
        public new ActivitySendExclusiveGatewayBuilder DestinataryRequester()
        {
            return (ActivitySendExclusiveGatewayBuilder)base.DestinataryRequester();
        }
    }
}
