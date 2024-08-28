using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner;
using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders
{
    public abstract class FieldBaseBuilder : BaseBuilder
    {
        protected FieldBaseBuilder(ContextBuilder context, IBaseBuilder parent, DataId fieldId) : base(context, parent)
        {
            FieldId = fieldId;
        }

        internal DataId FieldId { get; set; }

        protected FieldTypeEnum? _type;

        public virtual ActivityUserBuilder ActivityUser(DataId id = null)
        {
            return FindFirstParentOrThis<IActivityParent>().ActivityUser(id);
        }
        public virtual ActivitySendBuilder ActivitySend(DataId id = null)
        {
            return FindFirstParentOrThis<IActivityParent>().ActivitySend(id);
        }
        public virtual ActivitySignerBuilder ActivitySigner(DataId id = null)
        {
            return FindFirstParentOrThis<IActivityParent>().ActivitySigner(id);
        }
    }
}
