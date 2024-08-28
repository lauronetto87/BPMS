using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity
{
    public interface IActivityParent : IBaseBuilder
    {
        public ActivityUserBuilder ActivityUser(DataId id = null);
        public ActivitySendBuilder ActivitySend(DataId id = null);
        internal ExclusiveGatewayBuilder ExclusiveGateway(DataId id = null);
        public ActivitySignerBuilder ActivitySigner(DataId id = null);
    }
}
