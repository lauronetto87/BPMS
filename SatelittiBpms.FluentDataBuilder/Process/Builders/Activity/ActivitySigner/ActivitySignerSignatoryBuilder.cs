﻿using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Integration.Signer;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner
{
    public class ActivitySignerSignatoryBuilder : BaseBuilder
    {
        public ActivitySignerSignatoryBuilder(ContextBuilder context, IBaseBuilder parent) : base(context, parent)
        {
        }

        internal new ActivitySignerSignatoryData LastBuild => base.LastBuild as ActivitySignerSignatoryData;

        public ActivitySignerAuthorizerBuilder Authorizer()
        {
            return FindFirstParentOrThis<ActivitySignerBuilder>().Authorizer();
        }
        public ActivitySignerSignatoryBuilder Signatory()
        {
            return FindFirstParentOrThis<ActivitySignerBuilder>().Signatory();
        }

        private DataId _nameField;
        public ActivitySignerSignatoryBuilder NameField(DataId fieldId)
        {
            _nameField = fieldId;
            return this;
        }

        private DataId _cpfField;
        public ActivitySignerSignatoryBuilder CpfField(DataId fieldId)
        {
            _cpfField = fieldId;
            return this;
        }

        private DataId _emailField;
        public ActivitySignerSignatoryBuilder EmailField(DataId fieldId)
        {
            _emailField = fieldId;
            return this;
        }

        private DataId _originActivityId;
        public ActivitySignerSignatoryBuilder OriginActivity(DataId originActivityId)
        {
            _originActivityId = originActivityId;
            return this;
        }

        private SignerRegistrationLocationEnum? _registrationLocation;
        public ActivitySignerSignatoryBuilder RegistrationLocation(SignerRegistrationLocationEnum registrationLocation)
        {
            _registrationLocation = registrationLocation;
            return this;
        }

        internal override IData Build()
        {
            return new ActivitySignerSignatoryData()
            {
                RegistrationLocation = _registrationLocation ?? faker.Random.Enum<SignerRegistrationLocationEnum>(),
                SignatureTypeId = faker.Random.Int((int)SignatureTypeEnum.BOTH, (int)SignatureTypeEnum.INPERSON),
                SubscriberTypeId = faker.Random.Int((int)SubscriberTypeEnum.Signatory, (int)SubscriberTypeEnum.Authorizer),
                OriginActivityId = _originActivityId,
            };
        }
        protected override void AfterBuild(IData buildResult)
        {
            base.AfterBuild(buildResult);
            SetFields((ActivitySignerSignatoryData)buildResult);
        }

        private void SetFields(ActivitySignerSignatoryData activity)
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
