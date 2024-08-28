using SatelittiBpms.FluentDataBuilder.Process.Data;

namespace SatelittiBpms.FluentDataBuilder.Process.Helpers
{
    internal static class TreeVisitorHelper
    {
        public static ActivityUserData FindUserActivityThatComesBeforeExclusiveGateway(ExclusiveGatewayData branch)
        {
            var listOfActivity = branch.FindFirstParent<IActivityParentData>().Activities;
            for (int i = listOfActivity.Count - 1; i >= 0; i--)
            {
                if (listOfActivity[i] is ActivityUserData activityUser)
                {
                    return activityUser;
                }
            }
            return null;
        }
    }
}
