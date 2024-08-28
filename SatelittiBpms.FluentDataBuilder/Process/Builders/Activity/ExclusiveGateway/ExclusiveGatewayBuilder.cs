using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.FluentDataBuilder.Process.Helpers;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway
{
    public class ExclusiveGatewayBuilder : ActivityBaseBuilder
    {
        public ExclusiveGatewayBuilder(ContextBuilder context, BaseBuilder parent, DataId id) : base(context, parent, id)
        {
        }


        internal readonly List<ExclusiveGatewayBranchBuilder> _exclusiveGatewayBranchBuilders = new();
        public ExclusiveGatewayBranchBuilder Branch()
        {
            var builder = new ExclusiveGatewayBranchBuilder(Context, this);
            _exclusiveGatewayBranchBuilders.Add(builder);
            return builder;
        }

        internal new ExclusiveGatewayData LastBuild => base.LastBuild as ExclusiveGatewayData;
        internal override IData Build()
        {
            return new ExclusiveGatewayData
            {
                ActivityId = Id?.InternalId ?? ("Gateway_" + faker.Random.AlphaNumeric(7)),
                ActivityName = faker.Name.JobArea(),
                ActivityType = WorkflowActivityTypeEnum.EXCLUSIVE_GATEWAY_ACTIVITY,
                Branchs = _exclusiveGatewayBranchBuilders.Select(a => a.LastBuild).ToList(),
            };
        }

        protected override void AfterBuild(IData buildResult)
        {
            RemoveUnusedButtons((ExclusiveGatewayData)buildResult);
        }

        private static void RemoveUnusedButtons(ExclusiveGatewayData buildResult)
        {
            var activityUser = TreeVisitorHelper.FindUserActivityThatComesBeforeExclusiveGateway(buildResult);
            if (activityUser == null)
            {
                return;
            }
            activityUser.Buttons = activityUser.Buttons.Where(b => b.Branch != null).ToList();
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            foreach (var item in _exclusiveGatewayBranchBuilders)
            {
                yield return item;
            }
        }
    }
}