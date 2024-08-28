using SatelittiBpms.Models.BpmnIo;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SatelittiBpms.Services.Helpers
{
    public static class BpmnIoSerializerHelper
    {
        public static string Serializer(Definitions definitions)
        {
            var serializer = new XmlSerializer(typeof(Definitions));
            using var writer = new Utf8StringWriter();
            serializer.Serialize(writer, definitions, CreateNameSpaces());
            return writer.ToString();
        }

        public static Definitions Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof(Definitions));
            using var reader = new StringReader(xml);
            var definitions = (Definitions)serializer.Deserialize(reader);
            SetReferences(definitions);
            return definitions;
        }

        private static void SetReferences(Definitions definitions)
        {
            if (definitions.Process == null)
            {
                return;
            }
            foreach (var item in definitions.Process.EndEvent)
            {
                item.Parent = definitions.Process;
            }
            foreach (var item in definitions.Process.ExclusiveGateway)
            {
                item.Parent = definitions.Process;
            }
            foreach (var item in definitions.Process.SatelittiSigner)
            {
                item.Parent = definitions.Process;
            }
            foreach (var item in definitions.Process.SendTask)
            {
                item.Parent = definitions.Process;
            }
            foreach (var item in definitions.Process.StartEvent)
            {
                item.Parent = definitions.Process;
            }
            foreach (var item in definitions.Process.UserTask)
            {
                item.Parent = definitions.Process;
            }
        }
       

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }


        private static XmlSerializerNamespaces CreateNameSpaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsd", NameSpace.xsd);
            namespaces.Add("xsi", NameSpace.xsi);
            namespaces.Add("bpmn2", NameSpace.bpmn2);
            namespaces.Add("bpmndi", NameSpace.bpmndi);
            namespaces.Add("dc", NameSpace.dc);
            namespaces.Add("satelitti", NameSpace.satelitti);
            namespaces.Add("di", NameSpace.di);
            namespaces.Add("targetNamespace", "http://bpmn.io/schema/bpmn");
            namespaces.Add("schemaLocation", "http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd");
            return namespaces;
        }
    }
}
