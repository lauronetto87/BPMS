using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner
{
    public class ActivitySignerAuthorizerBuilder : BaseBuilder
    {
        public ActivitySignerAuthorizerBuilder(ContextBuilder context, IBaseBuilder parent) : base(context, parent)
        {
        }

        internal new ActivitySignerAuthorizerData LastBuild => base.LastBuild as ActivitySignerAuthorizerData;

        public ActivitySignerSignatoryBuilder Signatory()
        {
            return FindFirstParentOrThis<ActivitySignerBuilder>().Signatory();
        }

        public ActivitySignerAuthorizerBuilder Authorizer()
        {
            return FindFirstParentOrThis<ActivitySignerBuilder>().Authorizer();
        }

        private DataId _nameField;
        public ActivitySignerAuthorizerBuilder NameField(DataId fieldId)
        {
            _nameField = fieldId;
            return this;
        }

        private DataId _cpfField;
        public ActivitySignerAuthorizerBuilder CpfField(DataId fieldId)
        {
            _cpfField = fieldId;
            return this;
        }

        private DataId _emailField;
        public ActivitySignerAuthorizerBuilder EmailField(DataId fieldId)
        {
            _emailField = fieldId;
            return this;
        }

        private DataId _originActivityId;
        public ActivitySignerAuthorizerBuilder OriginActivity(DataId originActivityId)
        {
            _originActivityId = originActivityId;
            return this;
        }

        private SignerRegistrationLocationEnum? _registrationLocation;
        public ActivitySignerAuthorizerBuilder RegistrationLocation(SignerRegistrationLocationEnum registrationLocation)
        {
            _registrationLocation = registrationLocation;
            return this;
        }


        internal override IData Build()
        {
            return new ActivitySignerAuthorizerData()
            {
                RegistrationLocation = _registrationLocation ?? faker.Random.Enum<SignerRegistrationLocationEnum>(),
                OriginActivityId = _originActivityId?.InternalId,
            };
        }
        protected override void AfterBuild(IData buildResult)
        {
            base.AfterBuild(buildResult);
            SetFields((ActivitySignerAuthorizerData)buildResult);
        }

        private void SetFields(ActivitySignerAuthorizerData activity)
        {
            var allFields = activity.FindFirstParent<ProcessVersionData>().AllFields;
            if (_nameField != null)
            {
                var field = allFields.FirstOrDefault(f => f.Id.InternalId == _nameField.InternalId);
                activity.NameField = field ?? throw new System.ArgumentException($"Campo {nameof(activity.NameField)} não foi registrado no processo para ser utilizado na integração.");
            }
            if (_emailField != null)
            {
                var field = allFields.FirstOrDefault(f => f.Id.InternalId == _emailField.InternalId);
                activity.EmailField = field ?? throw new System.ArgumentException($"Campo {nameof(activity.EmailField)} não foi registrado no processo para ser utilizado na integração.");
            }
            if (_cpfField != null)
            {
                var field = allFields.FirstOrDefault(f => f.Id.InternalId == _cpfField.InternalId);
                activity.CpfField = field ?? throw new System.ArgumentException($"Campo {nameof(activity.CpfField)} não foi registrado no processo para ser utilizado na integração.");
            }
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            return new List<IBaseBuilder>();
        }
    }
}
