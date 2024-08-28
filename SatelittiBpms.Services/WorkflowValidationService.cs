using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Xml;

namespace SatelittiBpms.Services
{
    public class WorkflowValidationService : FluentValidationServiceBase, IWorkflowValidationService
    {
        private readonly XmlNamespaceManager nsmgr;

        public WorkflowValidationService()
        {
            NameTable nameTable = new NameTable();
            nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace(XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX, XmlDiagramConstants.BPMN2_NAMESPACE_URI);
            nsmgr.AddNamespace(XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX, XmlDiagramConstants.SATELITTI_NAMESPACE_URI);
        }

        public ResultContent Validate(XmlNode xmlProcessNode)
        {
            ValidateStartEvent(xmlProcessNode);
            ValidateEndEvent(xmlProcessNode);
            ValidateUserTasks(xmlProcessNode);
            ValidateSendTasks(xmlProcessNode);
            ValidateExclusiveGateways(xmlProcessNode);
            ValidateSignerIntegrationTasks(xmlProcessNode);
            return ValidationResult.Errors.Count > 0 ? Result.Error(ValidationResult) : Result.Success();
        }

        #region StartEvent Validations
        private void ValidateStartEvent(XmlNode xmlProcessNode)
        {
            var startEventNodeList = xmlProcessNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.START_EVENT_ACTIVITY}", nsmgr);
            if (startEventNodeList.Count <= 0)
            {
                AddErrors(XmlDiagramConstants.START_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_MISSING_START_EVENT);
                return;
            }
            if (startEventNodeList.Count > 1)
            {
                AddErrors(XmlDiagramConstants.START_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_TOO_MANY_START_EVENTS);
                return;
            }
            ValidateStartEventOutgoing(startEventNodeList[0]);
        }

        private void ValidateStartEventOutgoing(XmlNode startEventNode)
        {
            var outgoingNodes = startEventNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNodes.Count <= 0)
                AddErrors(XmlDiagramConstants.START_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_START_EVENT_MISSING_OUTGOING);
            if (outgoingNodes.Count > 1)
                AddErrors(XmlDiagramConstants.START_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_START_EVENT_HAVE_MORE_THAN_ONE_OUTGOING);
        }
        #endregion

        #region EndEvent Validations
        private void ValidateEndEvent(XmlNode xmlProcessNode)
        {
            var endEventNodeList = xmlProcessNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.END_EVENT_ACTIVITY}", nsmgr);
            if (endEventNodeList.Count <= 0)
                AddErrors(XmlDiagramConstants.END_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_MISSING_END_EVENT);
            ValidateEndEventIncoming(endEventNodeList);
        }

        private void ValidateEndEventIncoming(XmlNodeList endEventNodeList)
        {
            foreach (XmlNode endEvent in endEventNodeList)
            {
                if (endEvent.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr).Count <= 0)
                    AddErrors(XmlDiagramConstants.END_EVENT_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_END_EVENT_MISSING_INCOMING, new { id = (endEvent.Attributes["name"] ?? endEvent.Attributes["id"])?.Value });
            }
        }
        #endregion

        #region UserTask Validations
        private void ValidateUserTasks(XmlNode xmlProcessNode)
        {
            foreach (XmlNode userTaskNode in xmlProcessNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.USER_TASK_ACTIVITY}", nsmgr))
            {
                ValidateUserTaskOptions(userTaskNode);
                ValidateUserTaskOutgoing(userTaskNode);
                ValidateUserTaskIncoming(userTaskNode);
                XmlAttribute executorTypeAttribute = userTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXECUTOR_TYPE_ATTRIBUTE}"];
                if (!ValidateUserTaskExecutorType(executorTypeAttribute, (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value))
                    continue;
                if ((UserTaskExecutorTypeEnum)Convert.ToInt32(executorTypeAttribute.Value) == UserTaskExecutorTypeEnum.ROLE)
                    ValidateUserTaskExecutorId(userTaskNode);
                if ((UserTaskExecutorTypeEnum)Convert.ToInt32(executorTypeAttribute.Value) == UserTaskExecutorTypeEnum.PERSON)
                    ValidatePersonTaskExecutorId(userTaskNode);
            }
        }

        private void ValidateUserTaskIncoming(XmlNode userTaskNode)
        {
            if (userTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr).Count <= 0)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_INCOMING, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
        }

        private void ValidateUserTaskOutgoing(XmlNode userTaskNode)
        {
            var outgoingNodes = userTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNodes.Count <= 0)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OUTGOING, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
            if (outgoingNodes.Count > 1)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_HAVE_MORE_THAN_ONE_OUTGOING, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
        }

        private void ValidateUserTaskOptions(XmlNode userTaskNode)
        {
            var optionsNode = userTaskNode.SelectSingleNode($".//{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.TASK_OPTIONS_NODE_NAME}", nsmgr);
            if (optionsNode == null)
            {
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
                return;
            }
            var optionsList = optionsNode.SelectNodes($"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.TASK_OPTION_NODE_NAME}", nsmgr);
            if (optionsList.Count <= 0)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
        }

        private bool ValidateUserTaskExecutorType(XmlAttribute executorTypeAttribute, string taskName)
        {
            if (executorTypeAttribute == null)
            {
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_TYPE, new { id = taskName });
                return false;
            }

            return true;
        }

        private void ValidateUserTaskExecutorId(XmlNode userTaskNode)
        {
            if (userTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXECUTOR_ID_ATTRIBUTE}"] == null)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_ID, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
        }
        private void ValidatePersonTaskExecutorId(XmlNode userTaskNode)
        {
            if (userTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.PERSON_ID_ATTRIBUTE}"] == null)
                AddErrors(XmlDiagramConstants.USER_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_USER_TASK_MISSING_PERSON_ID, new { id = (userTaskNode.Attributes["name"] ?? userTaskNode.Attributes["id"])?.Value });
        }
        #endregion

        #region SendTask Validations
        private void ValidateSendTasks(XmlNode xmlProcessNode)
        {
            foreach (XmlNode sendTaskNode in xmlProcessNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.SEND_TASK_ACTIVITY}", nsmgr))
            {
                ValidateSendTaskOutgoing(sendTaskNode);
                ValidateSendTaskIncoming(sendTaskNode);
                XmlAttribute destinataryTypeAttribute = sendTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.DESTINATARY_TYPE_ATTRIBUTE}"];
                if (!ValidateSendTaskDestinataryType(destinataryTypeAttribute, (sendTaskNode.Attributes["name"] ?? sendTaskNode.Attributes["id"])?.Value))
                    continue;
                if ((SendTaskDestinataryTypeEnum)Convert.ToInt32(destinataryTypeAttribute.Value) != SendTaskDestinataryTypeEnum.REQUESTER)
                    ValidateSendTaskDestinataryId(sendTaskNode);
            }
        }

        private void ValidateSendTaskIncoming(XmlNode sendTaskNode)
        {
            if (sendTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr).Count <= 0)
                AddErrors(XmlDiagramConstants.SEND_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_INCOMING, new { id = (sendTaskNode.Attributes["name"] ?? sendTaskNode.Attributes["id"])?.Value });
        }

        private void ValidateSendTaskOutgoing(XmlNode sendTaskNode)
        {
            var outgoingNodes = sendTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNodes.Count <= 0)
                AddErrors(XmlDiagramConstants.SEND_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_OUTGOING, new { id = (sendTaskNode.Attributes["name"] ?? sendTaskNode.Attributes["id"])?.Value });
            if (outgoingNodes.Count > 1)
                AddErrors(XmlDiagramConstants.SEND_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_HAVE_MORE_THAN_ONE_OUTGOING, new { id = (sendTaskNode.Attributes["name"] ?? sendTaskNode.Attributes["id"])?.Value });
        }

        private bool ValidateSendTaskDestinataryType(XmlAttribute destinataryTypeAttribute, string taskName)
        {
            if (destinataryTypeAttribute == null)
            {
                AddErrors(XmlDiagramConstants.SEND_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_TYPE, new { id = taskName });
                return false;
            }
            return true;
        }

        private void ValidateSendTaskDestinataryId(XmlNode sendTaskNode)
        {
            if (sendTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.DESTINATARY_ID_ATTRIBUTE}"] == null
                && sendTaskNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.CUSTOM_EMAIL_ID_ATTRIBUTE}"] == null)
                AddErrors(XmlDiagramConstants.SEND_TASK_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_ID, new { id = (sendTaskNode.Attributes["name"] ?? sendTaskNode.Attributes["id"])?.Value });
        }
        #endregion

        #region ExclusiveGateway Validations

        private void ValidateExclusiveGateways(XmlNode xmlProcessNode)
        {
            foreach (XmlNode exclusiveGatewayNode in xmlProcessNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY}", nsmgr))
            {
                ValidateExclusiveGatewayIncoming(exclusiveGatewayNode);
                ValidateExclusiveGatewayOutgoing(xmlProcessNode, exclusiveGatewayNode);
            }
        }

        private void ValidateExclusiveGatewayOutgoing(XmlNode xmlProcessNode, XmlNode exclusiveGatewayNode)
        {
            var exclusiveGatewayOutgoings = exclusiveGatewayNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (exclusiveGatewayOutgoings.Count <= 0)
            {
                AddErrors(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_OUTGOING, new { id = (exclusiveGatewayNode.Attributes["name"] ?? exclusiveGatewayNode.Attributes["id"])?.Value });
                return;
            }

            foreach (XmlNode outgoing in exclusiveGatewayOutgoings)
            {
                var sequenceFlowNode = xmlProcessNode.SelectSingleNode($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.SEQUENCE_FLOW_NODE_NAME}[@id='{outgoing.InnerText}']", nsmgr);
                if (sequenceFlowNode == null || sequenceFlowNode.Attributes[$"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.OPTION_ATTRIBUTE}"] == null)
                {
                    AddErrors(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_OUTGOING_NOT_ASSOCIATED, new { id = (exclusiveGatewayNode.Attributes["name"] ?? exclusiveGatewayNode.Attributes["id"]).Value });
                    return;
                }
            }
        }

        private void ValidateExclusiveGatewayIncoming(XmlNode exclusiveGatewayNode)
        {
            if (exclusiveGatewayNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr).Count <= 0)
                AddErrors(XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY, ExceptionCodes.FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_INCOMING, new { id = (exclusiveGatewayNode.Attributes["name"] ?? exclusiveGatewayNode.Attributes["id"])?.Value });
        }
        #endregion

        #region SignerIntegration Validations
        private void ValidateSignerIntegrationTasks(XmlNode xmlProcessNode)
        {
            foreach (XmlNode signerIntegrationTask in xmlProcessNode.SelectNodes($"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.SIGNER_TASK}", nsmgr))
            {
                ValidateSignerIntegrationTaskOutgoing(signerIntegrationTask);
                ValidateSignerIntegrationTaskIncoming(signerIntegrationTask);
            }
        }

        private void ValidateSignerIntegrationTaskIncoming(XmlNode signerIntegrationTaskNode)
        {
            if (signerIntegrationTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.INCOMING_NODE_NAME}", nsmgr).Count <= 0)
                AddErrors(XmlDiagramConstants.SIGNER_TASK, ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_INCOMING, new { id = (signerIntegrationTaskNode.Attributes["name"] ?? signerIntegrationTaskNode.Attributes["id"])?.Value });
        }

        private void ValidateSignerIntegrationTaskOutgoing(XmlNode signerIntegrationTaskNode)
        {
            var outgoingNodes = signerIntegrationTaskNode.SelectNodes($"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            if (outgoingNodes.Count <= 0)
                AddErrors(XmlDiagramConstants.SIGNER_TASK, ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_OUTGOING, new { id = (signerIntegrationTaskNode.Attributes["name"] ?? signerIntegrationTaskNode.Attributes["id"])?.Value });
            if (outgoingNodes.Count > 1)
                AddErrors(XmlDiagramConstants.SIGNER_TASK, ExceptionCodes.FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_HAVE_MORE_THAN_ONE_OUTGOING, new { id = (signerIntegrationTaskNode.Attributes["name"] ?? signerIntegrationTaskNode.Attributes["id"])?.Value });
        }
        #endregion
    }
}
