using SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ActivitySigner;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.FluentDataBuilder.Process.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity.ExclusiveGateway
{
    public class ExclusiveGatewayBranchBuilder : BaseBuilder, IActivityParent
    {
        public ExclusiveGatewayBranchBuilder(ContextBuilder context, ExclusiveGatewayBuilder parent) : base(context, parent)
        {
        }

        internal readonly List<ActivityBaseBuilder> activityBuilders = new();
        public ActivityUserExclusiveGatewayBuilder ActivityUser(DataId id = null)
        {
            var activity = new ActivityUserExclusiveGatewayBuilder(Context, this, id);
            activityBuilders.Add(activity);
            return activity;
        }
        public ActivitySendExclusiveGatewayBuilder ActivitySend(DataId id = null)
        {
            var activity = new ActivitySendExclusiveGatewayBuilder(Context, this, id);
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
            return ExclusiveGateway(id);
        }
        ActivityUserBuilder IActivityParent.ActivityUser(DataId id)
        {
            return ActivityUser(id);
        }

        ActivitySendBuilder IActivityParent.ActivitySend(DataId id)
        {
            return ActivitySend(id);
        }

        public ActivitySignerBuilder ActivitySigner(DataId id = null)
        {
            var activity = new ActivitySignerBuilder(Context, this, id);
            activityBuilders.Add(activity);
            return activity;
        }


        internal new ExclusiveGatewayBranchData LastBuild => base.LastBuild as ExclusiveGatewayBranchData;
        internal override IData Build()
        {
            var parent = this;
            return new ExclusiveGatewayBranchData
            {
                Activities = activityBuilders.Select(a => (ActivityBaseData)a.LastBuild).ToList(),
                Button = null,
            };
        }

        protected override void AfterBuild(IData buildResult)
        {
            SetButtonBranch((ExclusiveGatewayBranchData)buildResult);
        }

        private void SetButtonBranch(ExclusiveGatewayBranchData branch)
        {
            var activityUser = TreeVisitorHelper.FindUserActivityThatComesBeforeExclusiveGateway((ExclusiveGatewayData)branch.Parent);
            if (activityUser == null)
            {
                return;
            }

            ButtonData button;

            var potentialButtons = activityUser.Buttons.Where(b => b.Branch == null).ToList();
            if (potentialButtons.Count == 0)
            {
                button = new ButtonData
                {
                    Description = faker.Random.Words()
                };
                activityUser.Buttons.Add(button);
            }
            else if (potentialButtons.Count == 1)
            {
                button = potentialButtons[0];
            }
            else
            {
                button = faker.PickRandom(potentialButtons);
            }
            branch.Button = button;
            button.Branch = branch;
        }

        protected override IEnumerable<IBaseBuilder> GetChildren()
        {
            foreach (var item in activityBuilders)
            {
                yield return item;
            }
        }

    }
}