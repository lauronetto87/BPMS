using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    [TestFixture]
    public class WildcardServiceFormatTest : BaseTest
    {
        private MockServices _mockServices;
        private IWildcardService _wildCardService;
        Data.FlowExecutedData _flowExecuted;

        [SetUp]
        public void Setup()
        {
            _mockServices = new MockServices();
        }

        [Test(Description = "Efetua o teste com todas as regras para a troca dos curingas.")]
        public async Task TestAllRulesToPerformWildcardSwap()
        {
            var textFieldId = new DataId();
            var currencyId = new DataId();
            var currencyValueId = new DataId();
            var dateTimeId = new DataId();
            var dateTimeValueId = new DataId();
            var emailId = new DataId();
            var fileId = new DataId();
            var numberId = new DataId();
            var checkboxId = new DataId();
            var textAreaId = new DataId();

            var dateTimeValue = DateTime.UtcNow;
            var currencyValue = "9999,99";

            var data = await _mockServices
                .BeginCreateProcess()
                    .ActivityUser()
                        .Field(dateTimeValueId)
                            .Type(FieldTypeEnum.DATETIME)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(dateTimeId)
                            .Type(FieldTypeEnum.DATETIME)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(currencyValueId)
                            .Type(FieldTypeEnum.CURRENCY)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(currencyId)
                            .Type(FieldTypeEnum.CURRENCY)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(emailId)
                            .Type(FieldTypeEnum.EMAIL)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(fileId)
                            .Type(FieldTypeEnum.FILE)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(numberId)
                            .Type(FieldTypeEnum.NUMBER)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(textAreaId)
                            .Type(FieldTypeEnum.TEXTAREA)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(textFieldId)
                            .Type(FieldTypeEnum.TEXTFIELD)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(checkboxId)
                            .Type(FieldTypeEnum.CHECKBOX)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteTask()
                        .FieldValue(dateTimeValueId, dateTimeValue)
                        .FieldValue(dateTimeId)
                        .FieldValue(currencyValueId, currencyValue)
                        .FieldValue(currencyId)
                .EndCreateFlows()
                .ExecuteInDataBase();

            _flowExecuted = data.FlowsExecuted[0];

            _wildCardService = _mockServices.GetService<IWildcardService>();

            var fielValues = _flowExecuted.FlowData.Tasks[0].FieldValues;

            Assert.AreEqual(FormatWildCard(emailId), GetFieldValue(emailId, fielValues).ToString());

            Assert.AreEqual(FormatWildCard(numberId), GetFieldValue(numberId, fielValues).ToString());

            Assert.AreEqual(FormatWildCard(textAreaId), GetFieldValue(textAreaId, fielValues).ToString());

            Assert.AreEqual(FormatWildCard(textFieldId), GetFieldValue(textFieldId, fielValues).ToString());

            Assert.AreEqual(FormatWildCard(dateTimeId), GetFieldValue(dateTimeId, fielValues)?.ToString() ?? "");
            Assert.AreEqual(FormatWildCard(dateTimeValueId),  dateTimeValue.ToString("dd/MM/yyyy"));

            Assert.AreEqual(FormatWildCard(currencyId), GetFieldValue(currencyId, fielValues)?.ToString() ?? "");
            Assert.AreEqual(FormatWildCard(currencyValueId), "R$ 9.999,99");

            Assert.AreEqual(FormatWildCard(checkboxId), (((bool)GetFieldValue(checkboxId, fielValues)) ? "Sim" : "Não"));

            var files = GetFieldValue(fileId, fielValues) as IList<FileToFieldValueDTO>;
            Assert.AreEqual(FormatWildCard(fileId), string.Join(", ", files.Select(f => f.FileName)));
        }

        private static object GetFieldValue(DataId fieldId, IList<FlowFieldValue> fielValues)
        {
            return fielValues.FirstOrDefault(field => field.FieldId.InternalId == fieldId.InternalId).Value;
        }

        private string FormatWildCard(DataId fieldId)
        {
            return _wildCardService.FormatDescriptionWildcard(WildcardHelper.MakeWildcardField(fieldId.InternalId), _flowExecuted.FlowInfo, null);
        }
    }
}
