using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;
using System.Linq;

namespace SatelittiBpms.Test.Extensions
{
    public static class ActivityUserDataExtension
    {
        public static ActivityDTO AsDto(this ActivityUserData activityUserData, ProcessVersionData processVersionData)
        {
            var fields = activityUserData.Fields.Select(f => f.AsDto()).ToList();

            var fieldsNotContainsInActivity = processVersionData
                .AllFields
                .Where(pf => fields.All(f => pf.Id.InternalId != f.FieldId));

            fields.AddRange(fieldsNotContainsInActivity.Select(f => f.AsDto()));

            return new ActivityDTO
            {
                ActivityId = activityUserData.ActivityId,
                ActivityName = activityUserData.ActivityName,
                ActivityType = activityUserData.ActivityType,
                ProcessVersionId = 0,
                Fields = fields,
            };
        }
    }
}
