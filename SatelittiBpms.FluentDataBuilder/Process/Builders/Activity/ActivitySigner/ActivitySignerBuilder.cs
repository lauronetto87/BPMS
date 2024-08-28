using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Integration.Signer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner
{
    public class ActivitySignerBuilder : ActivityBaseBuilder
    {
        public ActivitySignerBuilder(ContextBuilder context, IBaseBuilder parent, DataId id) : base(context, parent, id)
        {
        }

        internal readonly List<ActivitySignerSignatoryBuilder> _signatoriesBuilders = new();
        public ActivitySignerSignatoryBuilder Signatory()
        {
            var builder = new ActivitySignerSignatoryBuilder(Context, this);
            _signatoriesBuilders.Add(builder);
            return builder;
        }

        internal readonly List<ActivitySignerAuthorizerBuilder> _authorizersBuilders = new();
        public ActivitySignerAuthorizerBuilder Authorizer()
        {
            var builder = new ActivitySignerAuthorizerBuilder(Context, this);
            _authorizersBuilders.Add(builder);
            return builder;
        }


        private string _envelopeTitle;
        public ActivitySignerBuilder EnvelopeTitle(string envelopeTitle)
        {
            _envelopeTitle = envelopeTitle;
            return this;
        }


        private DataId _expirationDateField;
        public ActivitySignerBuilder ExpirationDateField(DataId fieldId)
        {
            _expirationDateField = fieldId;
            return this;
        }

        private DataId[] _filesField;
        public ActivitySignerBuilder Files(params DataId[] fieldsId)
        {
            _filesField = fieldsId;
            return this;
        }


        internal new ActivityBaseData LastBuild => base.LastBuild as ActivitySignerData;
        internal override IData Build()
        {
            return new ActivitySignerData
            {
                ActivityId = Id?.InternalId ?? ("Activity_" + faker.Random.AlphaNumeric(7)),
                ActivityName = faker.Name.JobArea(),
                ActivityType = WorkflowActivityTypeEnum.SIGNER_TASK,
                EnvelopeTitle = _envelopeTitle ?? faker.Random.Words(),
                Language = faker.PickRandom(new[] { 1, 2, 3 }),
                Segment = faker.PickRandom(new[] { 1, 2, 3, 4 }),
                SendReminders = (int)faker.PickRandom<ReminderFrequencyEnum>(),
                SignatoryAccessAuthentication = faker.Random.Bool(),
                AuthorizeEnablePriorAuthorizationOfTheDocument = _authorizersBuilders.Count > 0,
                AuthorizeAccessAuthentication = faker.Random.Bool(),
                Signatories = _signatoriesBuilders.Select(a => a.LastBuild).ToList(),
                Authorizers = _authorizersBuilders.Select(a => a.LastBuild).ToList(),
            };
        }

        protected override void AfterBuild(IData buildResult)
        {
            base.AfterBuild(buildResult);
            SetFields((ActivitySignerData)buildResult);
        }

        private void SetFields(ActivitySignerData activity)
        {
            if (_expirationDateField != null)
            {
                var field = activity.FindFirstParent<ProcessVersionData>().AllFields.FirstOrDefault(f => f.Id.InternalId == _expirationDateField.InternalId);
                activity.ExpirationDateField = field ?? throw new ArgumentException($"Campo {nameof(activity.ExpirationDateField)} não foi registrado no processo para ser utilizado na integração.");
            }

            if (_filesField != null && _filesField.Length > 0)
            {
                var files = new List<FieldBaseData>();
                foreach (var item in _filesField)
                {
                    var field = activity.FindFirstParent<ProcessVersionData>().AllFields.FirstOrDefault(f => f.Id.InternalId == item.InternalId);
                    if (field == null)
                    {
                        throw new System.ArgumentException($"Campo {nameof(activity.FileField)} não foi registrado no processo para ser utilizado na integração.");
                    }
                    files.Add(field);
                }
                activity.FileField = files;
            }
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            foreach (var item in _signatoriesBuilders)
            {
                yield return item;
            }
            foreach (var item in _authorizersBuilders)
            {
                yield return item;
            }
        }
    }
}