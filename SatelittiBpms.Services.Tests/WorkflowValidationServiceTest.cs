using NUnit.Framework;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.HandleException;
using System.Linq;
using System.Xml;

namespace SatelittiBpms.Services.Tests
{
    public class WorkflowValidationServiceTest
    {
        [Test]
        public void ensureThatIsNotValidWhenNotHaveStartEvent()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_withoutStartEvent));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_MISSING_START_EVENT, result.ValidationResult.Errors.Select(x => x.ErrorMessage).ToList());
            Assert.Contains(XmlDiagramConstants.START_EVENT_ACTIVITY, result.ValidationResult.Errors.Select(x => x.PropertyName).ToList());
        }

        [Test]
        public void ensureThatIsNotValidWhenHaveMoreThanOneStartEvent()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_withMoreThanOneStartEvent));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_TOO_MANY_START_EVENTS, result.ValidationResult.Errors.Select(x => x.ErrorMessage).ToList());
            Assert.Contains(XmlDiagramConstants.START_EVENT_ACTIVITY, result.ValidationResult.Errors.Select(x => x.PropertyName).ToList());
        }

        [Test]
        public void ensureThatIsNotValidWhenHaveMoreThanOneStartEventPath()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_withMoreThanOneStartEventPath));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_START_EVENT_HAVE_MORE_THAN_ONE_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void ensureThatIsValidWhenUserTaskHaveRequesterExecutorAndOptionAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withRequesterExecutorAndOptionAssocieted));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsValidWhenUserTaskHaveRoleExecutorAndOptionAssociated()
        {
            WorkflowValidationService wfService = new();
            XmlDocument bpmnXml = new();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withRoleExecutorAndOptionAssocieted));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoExecutorTypeAndHaveOptionAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorTypeAssocieted));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("TarefaUsuario", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoExecutorTypeAndHaveOptionAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorTypeAssocietedAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0hehwos", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoExecutorTypeAndHaveOptionAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorTypeAssocietedAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveRoleAndHaveOptionAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("TarefaUsuario", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveRoleAndHaveOptionAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorIdAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0h8jknc", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveRoleAndHaveOptionAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutExecutorIdAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionAssocieted));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("TarefaUsuario", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionAssocietedAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0hehwos", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionAssocietedAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionsAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionsAssocieted));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("TarefaUsuario", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionsAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionsAssocietedAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0hehwos", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveExecutorAndNoHaveOptionsAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withoutOptionsAssocietedAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsValidWhenSendTaskHaveRequesterDestinataryAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withRequesterDestinatary));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsValidWhenSendTaskHaveRoleDestinataryAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withRoleDestinatary));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryTypeAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryType));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("SendMail", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryTypeAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryTypeAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_1bjw19l", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryTypeAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryTypeAndNameId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_TYPE, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryIdAssociated()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("SendMail", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryIdAssociatedAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryIdAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0s2yyop", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveDestinataryIdAssociatedAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withoutDestinataryIdAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_ID, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsValidWhenAllTasksAreConnected()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_allTasksConnected));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsNotValidWhenStartEventNoHaveOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_startEventWithoutOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_START_EVENT_MISSING_OUTGOING, result.ValidationResult.Errors.Select(x => x.ErrorMessage).ToList());
            Assert.Contains(XmlDiagramConstants.START_EVENT_ACTIVITY, result.ValidationResult.Errors.Select(x => x.PropertyName).ToList());
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Atv4", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveOutgoingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutOutgoingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_1v9lsrk", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveOutgoingAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutOutgoingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveIncoming()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutIncoming));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Atv3", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveIncomingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutIncomingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0klc82s", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskNoHaveIncomingAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_userTaskWithoutIncomingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Send3", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveOutgoingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutOutgoingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_0jmjtr4", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveOutgoingAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutOutgoingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveIncoming()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutIncoming));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Send5", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveIncomingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutIncomingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Activity_093qq6s", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskNoHaveIncomingAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_sendTaskWithoutIncomingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenNotHaveEndEvent()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_withoutEndEvent));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_MISSING_END_EVENT, result.ValidationResult.Errors.Select(x => x.ErrorMessage).ToList());
            Assert.Contains(XmlDiagramConstants.END_EVENT_ACTIVITY, result.ValidationResult.Errors.Select(x => x.PropertyName).ToList());
        }

        [Test]
        public void ensureThatIsNotValidWhenEndEventNotHaveIncomingAndHaveNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_endEventWithoutIncoming));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_END_EVENT_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(2, errors.Count);
            Assert.Contains(XmlDiagramConstants.END_EVENT_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Fim1", attemptedProperty.GetValue(errors[0].AttemptedValue));
            Assert.AreEqual("Fim2", attemptedProperty.GetValue(errors[1].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenEndEventNotHaveIncomingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_endEventWithoutIncomingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_END_EVENT_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.END_EVENT_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Event_0jygx0d", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenEndEventNotHaveIncomingAndNameAndIdAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_endEventWithoutIncomingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_END_EVENT_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.END_EVENT_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveIncoming()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutIncoming));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList()); ;
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Qual opção?", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveIncomingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutIncomingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Gateway_0vwsqzt", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveIncomingAndNameAndIdAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutIncomingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Qual opção?", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveOutgoingAndNameAttribute()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutOutgoingAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Gateway_0vwsqzt", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayNotHaveOutgoingAndNameAndIdAttributes()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayWithoutOutgoingAndNameAndId));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.IsNull(attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayOutputsNotHaveAssociation()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayOutgoingNotHaveAssociation));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_OUTGOING_NOT_ASSOCIATED, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList()); ;
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Qual opção?", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayOutputsNotHaveAssociationAndName()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayOutgoingNotHaveAssociationAndName));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_OUTGOING_NOT_ASSOCIATED, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList()); ;
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Gateway_1qdpevr", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenExclusiveGatewayOutputsNotHaveSequenceFlow()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_exclusiveGatewayOutgoingNotHaveSequenceFlow));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_OUTGOING_NOT_ASSOCIATED, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, errors.Select(x => x.PropertyName).ToList()); ;
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Gateway_1qdpevr", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenUserTaskHaveMoreThanOneOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.userTaskTest_withTwoOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_USER_TASK_HAVE_MORE_THAN_ONE_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.USER_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Tarefa 2", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSendTaskHaveMoreThanOneOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.sendTaskTest_withTwoOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_HAVE_MORE_THAN_ONE_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SEND_TASK_ACTIVITY, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Tarefa 4", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSignerIntegrationTaskNoHaveOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_ssignIntegrationTaskWithoutOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SIGNER_TASK, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Tarefa 3", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }


        [Test]
        public void ensureThatIsNotValidWhenSignerIntegrationTaskNoHaveIncoming()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_ssignIntegrationTaskWithoutIncoming));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_INCOMING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SIGNER_TASK, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Tarefa 3", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }

        [Test]
        public void ensureThatIsNotValidWhenSignerIntegrationTaskHaveMoreThanOneOutgoing()
        {
            WorkflowValidationService wfService = new WorkflowValidationService();
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramTest_ssignIntegrationTaskWithTwoOutgoing));
            var result = wfService.Validate(bpmnXml.GetElementsByTagName("bpmn2:process")[0]);
            Assert.IsFalse(result.Success);
            var errors = result.ValidationResult.Errors;
            Assert.Contains(ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_HAVE_MORE_THAN_ONE_OUTGOING, errors.Select(x => x.ErrorMessage).ToList());
            Assert.AreEqual(1, errors.Count);
            Assert.Contains(XmlDiagramConstants.SIGNER_TASK, errors.Select(x => x.PropertyName).ToList());
            var attemptedType = errors[0].AttemptedValue.GetType();
            var attemptedProperty = attemptedType.GetProperty("id");
            Assert.AreEqual("Tarefa 3", attemptedProperty.GetValue(errors[0].AttemptedValue));
        }
    }
}
