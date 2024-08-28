using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Test.Extensions
{
    public static class ActivityFieldDataExtension
    {
        public static ActivityFieldDTO AsDto(this ActivityFieldData fieldData)
        {
            return new ActivityFieldDTO
            {
                FieldId = fieldData.Id.InternalId,
                FieldLabel = fieldData.Label,
                FieldType = fieldData.Type,
                State = fieldData.State,
                ProcessVersionId = 0,
                SystemFieldId = 0,
                TaskId = 0,
            };
        }
    }
}
