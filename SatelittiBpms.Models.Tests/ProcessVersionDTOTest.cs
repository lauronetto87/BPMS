using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Tests
{
    public class ProcessVersionDTOTest
    {
        [Test]
        public void ensureThatNameCanNotBeNull()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO() { Name = null };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.PROCESS_VERSION_NAME_REQUIRED)));
        }

        [Test]
        public void ensureThatNameCanNotBeEmpty()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO() { Name = String.Empty };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.PROCESS_VERSION_NAME_REQUIRED)));
        }

        [Test]
        public void ensureThatRolesIdsCanNotBeEmptyWhenNeedPublish()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO()
            {
                NeedPublish = true,
                RolesIds = new List<int>()
            };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("RolesIds")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.PROCESS_VERSION_ROLES_REQUIRED)));
        }

        [Test]
        public void ensureThatRolesIdsCanBeEmptyWhenNotNeedPublish()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO()
            {
                Name = "ProcessName",
                NeedPublish = false,
                RolesIds = new List<int>()
            };
            var result = dto.Validate();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ensureThatDiagramContentCanNotBeEmptyWhenNeedPublish()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO()
            {
                Name = "ProcessName",
                NeedPublish = true,
                DiagramContent = String.Empty,
                RolesIds = new List<int>()
            };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("DiagramContent")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.PROCESS_VERSION_CONTENT_DIAGRAM_REQUIRED)));
        }

        [Test]
        public void ensureThatDiagramContentCanBeEmptyWhenNotNeedPublish()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO()
            {
                Name = "ProcessName",
                NeedPublish = false,
                DiagramContent = String.Empty,
                RolesIds = new List<int>()
            };
            var result = dto.Validate();
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ensureThatCanBeValid()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO()
            {
                Name = "someName",
                Description = "Process",
                DescriptionFlow = "Flow Description ",
                DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"><bpmn2:startEvent id=\"StartEvent_1\"><bpmn2:outgoing>Flow_0ptfxvq</bpmn2:outgoing></bpmn2:startEvent><bpmn2:userTask id=\"Activity_0tzn5gu\"><bpmn2:incoming>Flow_0ptfxvq</bpmn2:incoming><bpmn2:outgoing>Flow_080rojc</bpmn2:outgoing></bpmn2:userTask><bpmn2:sequenceFlow id=\"Flow_0ptfxvq\" sourceRef=\"StartEvent_1\" targetRef=\"Activity_0tzn5gu\" /><bpmn2:endEvent id=\"Event_0usorsq\"><bpmn2:incoming>Flow_080rojc</bpmn2:incoming></bpmn2:endEvent><bpmn2:sequenceFlow id=\"Flow_080rojc\" sourceRef=\"Activity_0tzn5gu\" targetRef=\"Event_0usorsq\" /></bpmn2:process><bpmndi:BPMNDiagram id=\"BPMNDiagram_1\"><bpmndi:BPMNPlane id=\"BPMNPlane_1\" bpmnElement=\"Process_1\"><bpmndi:BPMNEdge id=\"Flow_0ptfxvq_di\" bpmnElement=\"Flow_0ptfxvq\"><di:waypoint x=\"448\" y=\"258\" /><di:waypoint x=\"500\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNEdge id=\"Flow_080rojc_di\" bpmnElement=\"Flow_080rojc\"><di:waypoint x=\"600\" y=\"258\" /><di:waypoint x=\"652\" y=\"258\" /></bpmndi:BPMNEdge><bpmndi:BPMNShape id=\"_BPMNShape_StartEvent_2\" bpmnElement=\"StartEvent_1\"><dc:Bounds x=\"412\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Activity_0tzn5gu_di\" bpmnElement=\"Activity_0tzn5gu\"><dc:Bounds x=\"500\" y=\"218\" width=\"100\" height=\"80\" /></bpmndi:BPMNShape><bpmndi:BPMNShape id=\"Event_0usorsq_di\" bpmnElement=\"Event_0usorsq\"><dc:Bounds x=\"652\" y=\"240\" width=\"36\" height=\"36\" /></bpmndi:BPMNShape></bpmndi:BPMNPlane></bpmndi:BPMNDiagram></bpmn2:definitions>",
                FormContent = "[{\"label\":\"Text Field\",\"placeholder\":\"\",\"tooltip\":\"\",\"widget\":{\"type\":\"input\"},\"type\":\"textfield\",\"input\":true,\"key\":\"textField\",\"prefix\":\"\",\"customClass\":\"\",\"suffix\":\"\",\"multiple\":false,\"defaultValue\":null,\"protected\":false,\"unique\":false,\"persistent\":true,\"hidden\":false,\"clearOnHide\":true,\"refreshOn\":\"\",\"redrawOn\":\"\",\"tableView\":true,\"modalEdit\":false,\"dataGridLabel\":false,\"labelPosition\":\"top\",\"description\":\"\",\"errorLabel\":\"\",\"hideLabel\":false,\"tabindex\":\"\",\"disabled\":false,\"autofocus\":false,\"dbIndex\":false,\"customDefaultValue\":\"\",\"calculateValue\":\"\",\"calculateServer\":false,\"attributes\":{},\"validateOn\":\"change\",\"validate\":{\"required\":false,\"custom\":\"\",\"customPrivate\":false,\"strictDateValidation\":false,\"multiple\":false,\"unique\":false,\"minLength\":\"\",\"maxLength\":\"\",\"pattern\":\"\"},\"conditional\":{\"show\":null,\"when\":null,\"eq\":\"\"},\"overlay\":{\"style\":\"\",\"left\":\"\",\"top\":\"\",\"width\":\"\",\"height\":\"\"},\"allowCalculateOverride\":false,\"encrypted\":false,\"showCharCount\":false,\"showWordCount\":false,\"properties\":{},\"allowMultipleMasks\":false,\"mask\":false,\"inputType\":\"text\",\"inputFormat\":\"plain\",\"inputMask\":\"\",\"spellcheck\":true,\"id\":\"eigoxw1d\"}]",
                RolesIds = new List<int>() { 1 }
            };
            var result = dto.Validate();
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ensureThatGetSetTenantId()
        {
            ProcessVersionDTO dto = new ProcessVersionDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }
    }
}
