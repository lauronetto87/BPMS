using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Test.Helpers
{
    public static class WildcardHelper
    {
        private static readonly Bogus.Faker faker = new();

        public static string Requester { get; } = "[[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]]";
        public static string FlowNumber { get; } = "[[{\"text\":\"Número Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]]";
        public static string Process { get; } = "[[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]]";
        public static string RequestDate { get; } = "[[{\"text\":\"Data da Solicitação\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]]";

        public static string MakeWildcardField(string componentIntenalId)
        {
            return $"[[{{\"text\":\"{faker.Person.FullName}\",\"value\":\"{componentIntenalId}\",\"prefix\":\"@\"}}]]";
        }

        public static WildcardTestData BuildWildcardTestData(params string[] fieldIds)
        {
            var wildcardTestData = new WildcardTestData();
            foreach (var fieldId in fieldIds)
            {
                wildcardTestData.AddField(fieldId);
            }
            return wildcardTestData;
        }

        public class WildcardTestData
        {
            private List<string> fieldIds = new();

            internal void AddField(string fieldId)
            {
                fieldIds.Add(fieldId);
            }


            private string _lastTemplateFixedBuild;

            public string BuildUserInput()
            {
                var lastTemplateFixedBuild = faker.Lorem.Paragraphs();
                _lastTemplateFixedBuild = lastTemplateFixedBuild;

                lastTemplateFixedBuild += FlowNumber;

                foreach (var fieldId in fieldIds)
                {
                    lastTemplateFixedBuild += " - " + MakeWildcardField(fieldId);
                }

                return lastTemplateFixedBuild;
            }

            public string BuildResult(IList<FlowFieldValue> fieldValues, int flowId)
            {
                var lastTemplateFixedBuild = _lastTemplateFixedBuild;
                lastTemplateFixedBuild += flowId;

                foreach (var fieldId in fieldIds)
                {
                    lastTemplateFixedBuild += " - " + fieldValues.FirstOrDefault(flowField => flowField.FieldId.InternalId == fieldId).Value;
                }
                return lastTemplateFixedBuild;
            }
        }
    }
}
