using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Test.Extensions
{
    public static class FieldBaseDataExtension
    {
        public static ActivityFieldDTO AsDto(this FieldBaseData fieldData)
        {
            if (fieldData is ProcessFieldData processFieldData)
            {
                return processFieldData.AsDto();
            }
            else if (fieldData is ActivityFieldData activityFieldData)
            {
                return activityFieldData.AsDto();
            }
            throw new System.Exception($"Não tratado o tipo {fieldData.GetType().Name} na conversão para {nameof(ActivityFieldDTO)}.");
        }
    }
}
