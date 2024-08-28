using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.FluentDataBuilder.Process.DiagramXml;
using SatelittiBpms.FluentDataBuilder.Process.FormJson;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Process
{
    public class ProcessBuilder : BaseBuilder, IActivityParent
    {
        public ProcessBuilder(ContextBuilder context) : base(context, null)
        {

        }

        internal readonly List<ActivityBaseBuilder> activityBuilders = new();
        public ActivityUserBuilder ActivityUser(DataId id = null)
        {
            var activity = new ActivityUserBuilder(Context, this, id);
            activityBuilders.Add(activity);
            return activity;
        }
        public ActivitySendBuilder ActivitySend(DataId id = null)
        {
            var activity = new ActivitySendBuilder(Context, this, id);
            activityBuilders.Add(activity);
            return activity;
        }
        public ExclusiveGatewayBuilder ExclusiveGateway(DataId id = null)
        {
            var gateway = new ExclusiveGatewayBuilder(Context, this, id);
            activityBuilders.Add(gateway);
            return gateway;
        }
        ExclusiveGatewayBuilder IActivityParent.ExclusiveGateway(DataId id)
        {
            var gateway = new ExclusiveGatewayBuilder(Context, this, id);
            activityBuilders.Add(gateway);
            return gateway;
        }
        public ActivitySignerBuilder ActivitySigner(DataId id = null)
        {
            var activity = new ActivitySignerBuilder(Context, this, id);
            activityBuilders.Add(activity);
            return activity;
        }

        internal readonly List<ProcessFieldBuilder> _processFieldBuilders = new();
        public ProcessFieldBuilder Field(DataId fieldId = null)
        {
            var builder = new ProcessFieldBuilder(Context, this, fieldId);
            _processFieldBuilders.Add(builder);
            return builder;
        }


        private string _descriptionFlow;
        public ProcessBuilder DescriptionFlow(string descriptionFlow)
        {
            _descriptionFlow = descriptionFlow;
            return this;
        }

        private bool _needPublish = true;
        public ProcessBuilder NoNeedPublish()
        {
            _needPublish = false;
            return this;
        }

        internal new ProcessVersionData LastBuild => base.LastBuild as ProcessVersionData;
        internal override IData Build()
        {
            var tenantId = faker.PickRandom(Context.Tenants.Select(p => p.Id));
            return new ProcessVersionData
            {
                Description = faker.Lorem.Paragraph(),
                DescriptionFlow = _descriptionFlow ?? faker.Lorem.Paragraph(),
                Name = faker.Name.JobType(),
                NeedPublish = _needPublish,
                Activities = activityBuilders.Select(a => (ActivityBaseData)a.LastBuild).ToList(),
                TaskSequance = activityBuilders.Count,
                RolesIds = faker.PickRandom(Context.Roles.Select(p => p.Id), Context.Roles.Count <= 2 ? 1 : 3).ToList(),
                TenantId = tenantId,
                DiagramContent = null,
                FormContent = null,
                Version = 0,
                Fields = _processFieldBuilders.Select(a => a.LastBuild).ToList(),
                ContextBuilder = Context,
            };
        }

        protected override void AfterBuild(IData buildResult)
        {
            var processVersion = (ProcessVersionData)buildResult;

            AdjustPropertiesOfActivityFieldsWithProcessFieldValues(processVersion);

            processVersion.FormContent = FormJsonHelper.Generate(LastBuild);

            processVersion.DiagramContent = DiagramXmlHelper.Generate(LastBuild);
        }

        private static void AdjustPropertiesOfActivityFieldsWithProcessFieldValues(ProcessVersionData processVersion)
        {
            var activitiyUserFields = processVersion.AllFields.ToList();
            var processVersionFields = processVersion.Fields;

            activitiyUserFields.RemoveAll(f => processVersionFields.Contains(f));

            foreach (var processField in processVersionFields)
            {
                if (processField.Id == null)
                {
                    continue;
                }

                foreach (var activitiyField in activitiyUserFields.Where(f => f.Id.InternalId == processField.Id.InternalId))
                {
                    activitiyField.Id = processField.Id;
                    activitiyField.Label = processField.Label;
                    activitiyField.Type = processField.Type;
                }
            }
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            foreach (var item in activityBuilders)
            {
                yield return item;
            }
            foreach (var item in _processFieldBuilders)
            {
                yield return item;
            }
        }
    }
}
