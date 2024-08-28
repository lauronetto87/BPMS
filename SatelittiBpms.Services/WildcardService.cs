using Newtonsoft.Json.Linq;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SatelittiBpms.Services
{
    public class WildcardService : IWildcardService
    {
        const string DELIMITER_INIT = "[[";
        const string DELIMITER_END = "]]";

        private readonly ITranslateService _translateService;


        public WildcardService(
            ITranslateService translateService)
        {
            _translateService = translateService;
        }

        public string FormatDescriptionWildcard(string description, FlowInfo flow, IList<SuiteUserViewModel> userViewModel)
        {
            if (string.IsNullOrEmpty(description)) return "";
            var arrayDescription = description.Split(DELIMITER_INIT).Where(x => x.Contains(DELIMITER_END)).Select(x => x.Substring(0, x.IndexOf(DELIMITER_END))).Distinct().ToList();

            foreach (string content in arrayDescription)
            {
                JObject json = JObject.Parse(content);

                if (json["prefix"].ToString().Equals("#"))
                    description = description.Replace(DELIMITER_INIT + content + DELIMITER_END, GetFlowInfo(flow, json["value"].ToString(), userViewModel));
                else
                    description = description.Replace(DELIMITER_INIT + content + DELIMITER_END, GetFieldValues(flow.Tasks.OrderByDescending(x => x.Id).FirstOrDefault(), json["value"].ToString()));
            }
            return description;
        }

        private string GetFlowInfo(FlowInfo flow, string wildcardFlowType, IList<SuiteUserViewModel> userViewModel)
        {
            switch (wildcardFlowType)
            {
                case "wildcards.requester":
                    return userViewModel?.FirstOrDefault(u => u.Id == flow.RequesterId).Name;
                case "wildcards.flowNumber":
                    return flow.Id.ToString();
                case "wildcards.process":
                    return flow.ProcessVersion.Name;
                case "wildcards.requestDate":
                    return flow.CreatedDate.ToString(_translateService.Localize("defaultFormat.dateTime"));
                default:
                    return "";
            }
        }

        private string GetFieldValues(TaskInfo taskInfo, string wildcardFlowType)
        {
            if (taskInfo.FieldsValues == null)
                return "";

            var fieldValue = taskInfo.FieldsValues.FirstOrDefault(x => x.Field.ComponentInternalId == wildcardFlowType);
            if (fieldValue == null) return "";

            var formIoJson = taskInfo.Flow.ProcessVersion.FormContent;

            var fieldData = GetFieldValue(formIoJson, wildcardFlowType);

            if (fieldData == null)
            {
                return "";
            }
            return GetFieldValueExibition(fieldValue.FieldValue, fieldData);
        }

        private JObject GetFieldValue(string formIoObj, string wildcardFlowType)
        {
            var components = FormIoHelper.GetAllComponents(formIoObj);
            return components.FirstOrDefault(i => i.Value<string>("key") == wildcardFlowType);
        }

        private string GetFieldValueExibition(string valueUserSelected, JObject field)
        {
            switch (field.Value<string>("type"))
            {
                case "select":
                    if (field.Value<bool>("multiple"))
                    {
                        if (valueUserSelected == null || valueUserSelected == "")
                        {
                            return "";
                        }
                        var userResponse = JArray.Parse(valueUserSelected);
                        var selections = userResponse.Select(i =>
                        {
                            var itemSelected = i.ToString();

                            var selectOption = field.Value<JObject>("data")?.Value<JArray>("values")?.Where(i => i.Value<string>("value") == itemSelected)?.FirstOrDefault();
                            if (selectOption == null)
                            {
                                return "";
                            }
                            return selectOption.Value<string>("label");
                        });
                        return string.Join(", ", selections);
                    }
                    else
                    {
                        var selectOption = field.Value<JObject>("data")?.Value<JArray>("values")?.Where(i => i.Value<string>("value") == valueUserSelected)?.FirstOrDefault();
                        if (selectOption == null)
                        {
                            return "";
                        }
                        return selectOption.Value<string>("label");
                    }

                case "radio":
                    var radioOption = field.Value<JArray>("values").Where(i => i.Value<string>("value") == valueUserSelected).FirstOrDefault();
                    if (radioOption == null)
                    {
                        return "";
                    }
                    return radioOption.Value<string>("label");
                case "checkbox":
                    return valueUserSelected == "True" ? _translateService.Localize("wildcards.translateUserInput.checkboxChecked") : _translateService.Localize("wildcards.translateUserInput.checkboxUnmarked");
                case "currency":
                    if (valueUserSelected == "") return "";
                    return (Convert.ToDouble(valueUserSelected)).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                case "datetime":
                    if (valueUserSelected == "") return "";
                    return Convert.ToDateTime(valueUserSelected).ToString(_translateService.Localize("defaultFormat.dateTime"));
                case "file":
                    if (valueUserSelected == "") return "";
                    var files = JArray.Parse(valueUserSelected).Select(i =>
                    {
                        return i.Value<string>("originalName");
                    });
                    return string.Join(", ", files);
                case "textfield":
                    return FieldValueInfoHelper.ValueToDisplay(valueUserSelected, Models.Enums.FieldTypeEnum.TEXTFIELD);
                case "textarea":
                default:
                    return valueUserSelected;
            }
        }       
    }
}
