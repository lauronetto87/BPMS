using Newtonsoft.Json.Linq;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using System;

namespace SatelittiBpms.Services.Helpers
{
    public static class FieldValueInfoHelper
    {
        public static T CovertType<T>(FieldValueInfo fieldValue)
        {
            var paramterType = typeof(T);
            if (paramterType == typeof(DateTime))
            {
                if (!DateTime.TryParse(fieldValue.FieldValue, out DateTime value))
                {
                    throw new ArgumentException($"O campo \"{fieldValue.Field.Name}\" não contém um valor válido, valor informado \"{fieldValue.FieldValue}\".");
                }
                return (T)(object)value;
            }
            if (paramterType == typeof(DateTime?))
            {
                if (string.IsNullOrWhiteSpace(fieldValue.FieldValue))
                {
                    return default;
                }
                if (!DateTime.TryParse(fieldValue.FieldValue, out DateTime value))
                {
                    throw new ArgumentException($"O campo \"{fieldValue.Field.Name}\" não contém um valor válido, valor informado \"{fieldValue.FieldValue}\".");
                }
                return (T)(object)value;
            }
            else if (paramterType == typeof(int))
            {
                if (!int.TryParse(fieldValue.FieldValue, out int value))
                {
                    throw new ArgumentException($"O campo \"{fieldValue.Field.Name}\" não contém um valor válido, valor informado \"{fieldValue.FieldValue}\".");
                }
                return (T)(object)value;
            }
            else if (paramterType == typeof(string))
            {
                return (T)(object)fieldValue.FieldValue;
            }
            throw new ArgumentException($"Não foi tratado a conversão de tipo para o tipo {paramterType.Name}");
        }

        public static string ValueToDisplay(string value, FieldTypeEnum type)
        {
            if (value == null)
            {
                return null;
            }
            switch (type)
            {
                case FieldTypeEnum.TEXTFIELD:
                    if (!value.TrimStart().StartsWith("{") || !value.TrimEnd().EndsWith("}"))
                    {
                        return value;
                    }
                    JObject valueJson;
                    try
                    {
                        valueJson = JObject.Parse(value);
                    }
                    catch
                    {
                        return value;
                    }
                    if (!valueJson.ContainsKey("value"))
                    {
                        return value;
                    }
                    return valueJson.Value<string>("value");
                default:
                    break;
            }
            return value;
        }
    }
}
