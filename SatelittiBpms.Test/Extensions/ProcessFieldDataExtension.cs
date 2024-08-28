using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Test.Extensions
{
    public static class ProcessFieldDataExtension
    {
        public static ActivityFieldDTO AsDto(this ProcessFieldData fieldData)
        {
            return new ActivityFieldDTO
            {
                FieldId = fieldData.Id.InternalId,
                FieldLabel = fieldData.Label,
                FieldType = fieldData.Type,
                State = Models.Enums.ProcessTaskFieldStateEnum.EDITABLE,
                ProcessVersionId = 0,
                SystemFieldId = 0,
                TaskId = 0,
            };
        }
    }
}
