using Newtonsoft.Json.Linq;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.FluentDataBuilder.Process.FormJson
{
    public static class FormJsonHelper
    {

        private static readonly Dictionary<FieldTypeEnum, string> _typeFieldMap = new()
        {
            { FieldTypeEnum.CHECKBOX, "checkbox" },
            { FieldTypeEnum.COLUMNS, "columns" },
            { FieldTypeEnum.CURRENCY, "currency" },
            { FieldTypeEnum.DATETIME, "datetime" },
            { FieldTypeEnum.EMAIL, "email" },
            { FieldTypeEnum.FILE, "file" },
            { FieldTypeEnum.NUMBER, "number" },
            { FieldTypeEnum.RADIO, "radio" },
            { FieldTypeEnum.SELECT, "select" },
            { FieldTypeEnum.TABLE, "table" },
            { FieldTypeEnum.TABS, "tabs" },
            { FieldTypeEnum.TEXTAREA, "textarea" },
            { FieldTypeEnum.TEXTFIELD, "textfield" },
            { FieldTypeEnum.CONTENT, "content" },
        };


        public static string Generate(ProcessVersionData processVersion)
        {
            var components = new List<JObject>();

            foreach (var fieldGrouping in processVersion.AllFields.GroupBy(f => f.Id.InternalId))
            {
                components.Add(BuildFormComponentJson(fieldGrouping.First()));
            }

            var jObject = new JObject()
            {
                {"components", JArray.FromObject(components) }
            };

            return jObject.ToString(Newtonsoft.Json.Formatting.None);
        }

        private static JObject BuildFormComponentJson(FieldBaseData field)
        {
            return new JObject
            {
                {"label", new JValue(field.Label) },
                {"type", new JValue(_typeFieldMap[field.Type]) },
                {"input", new JValue(true) },
                {"key", new JValue(field.Id.InternalId) },
                {"disabled", new JValue(false) },
                {"hidden", new JValue(false) },
                {"multiple", new JValue(false) },
                {"dataSrc", new JArray() },
                {
                    "validate",
                    new JObject
                    {
                        { "required", new JValue(false) }
                    }
                },
            };
        }

    }
}
