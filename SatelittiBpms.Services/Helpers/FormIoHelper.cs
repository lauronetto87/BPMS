using DotJEM.Json.Visitor;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.Helpers
{
    public static class FormIoHelper
    {
        public static List<JObject> GetAllComponents(string formIoJson)
        {
            if (string.IsNullOrWhiteSpace(formIoJson))
            {
                return new List<JObject>();
            }
            var formIoObj = JObject.Parse(formIoJson);
            return GetAllComponents(formIoObj);
        }

        public static List<JObject> GetAllComponents(JObject formIoJson)
        {
            var visitor = new Visitor();
            visitor.DoAccept(formIoJson, new NullJsonVisitorContext());
            return visitor.ToList();
        }

        private class Visitor : JsonVisitor<NullJsonVisitorContext>, IEnumerable<JObject>
        {
            private readonly List<JObject> objects = new List<JObject>();

            protected override void Visit(JObject objJson, NullJsonVisitorContext context)
            {
                if (objJson.Property("input")?.Value?.Value<bool?>() != null && objJson.Property("type") != null)
                {
                    objects.Add(objJson);
                }
                base.Visit(objJson, context);
            }

            public int Count => objects.Count;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<JObject> GetEnumerator() => objects.GetEnumerator();
        }
    }


}
