namespace SatelittiBpms.Models.HandleException
{
    public class ExceptionCodes
    {
        public static string PARAMETERS_VALIDATION_ERRORS = "exceptions.parametersValidationErrors";
        public static string MISSING_FUNCTION_TO_HANDLE = "exceptions.missingFunctionToHandle";
        public static string ENTITY_TO_UPDATE_NOT_FOUND = "exceptions.entityToUpdateNotFound";
        public static string ENTITY_TO_DELETE_NOT_FOUND = "exceptions.entityToDeleteNotFound";
        public static string ACTIVITY_TYPE_NOT_EXPECTED = "exceptions.activityTypeNotExpected";

        #region ROLES EXCEPTIONS
        public static string ROLE_NAME_REQUIRED = "exceptions.role.nameRequired";
        public static string ROLE_USERS_IDS_REQUIRED = "exceptions.role.userIdsRequired";
        public static string INSERT_ROLE_ERROR = "exceptions.role.insertRoleError";
        public static string INSERT_ROLE_TRANSACTION_ERROR = "exceptions.role.insertRoleTransactionError";
        public static string UPDATE_ROLE_TRANSACTION_ERROR = "exceptions.role.updateRoleTransactionError";
        public static string ROLE_NAME_ALREADY_EXISTS = "exceptions.role.roleNameAlreadyExists";
        #endregion

        #region PROCESS VERSION EXCEPTIONS
        public static string PROCESS_VERSION_NAME_REQUIRED = "exceptions.processVersion.nameRequired";
        public static string PROCESS_VERSION_ROLES_REQUIRED = "exceptions.processVersion.rolesIdsRequired";
        public static string PROCESS_VERSION_CONTENT_DIAGRAM_REQUIRED = "exceptions.processVersion.contentDiagramRequired";
        public static string PROCESS_VERSION_SAVE_GET_PROCESS_ERROR = "exceptions.processVersion.saveGetProcessError";
        public static string PROCESS_VERSION_SAVE_INSERT_PROCESS_ERROR = "exceptions.processVersion.saveInsertProcessError";
        public static string PROCESS_VERSION_SAVE_TRANSACTION_ERROR = "exceptions.processVersion.saveTransactionError";
        public static string PROCESS_VERSION_SAVE_NAME_DUPLICATE = "exceptions.processVersion.saveNameDuplicate";
        public static string MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR = "exceptions.processVersion.mandatoryFieldForIntegrationWithSignerError";
        public static string INTEGRATION_ACTIVITY_WITHOUT_CONFIGURING = "exceptions.processVersion.integrationActivityWithoutConfiguring";
        #endregion

        #region FLOW EXCEPTIONS
        #region START EVENT EXCEPTIONS
        public static string FLOW_DIAGRAM_MISSING_START_EVENT = "exceptions.process.flow.missingStartEvent";
        public static string FLOW_DIAGRAM_TOO_MANY_START_EVENTS = "exceptions.process.flow.tooManyStartEvents";
        public static string FLOW_DIAGRAM_START_EVENT_MISSING_OUTGOING = "exceptions.process.flow.startEventMissingOutgoing";
        public static string FLOW_DIAGRAM_START_EVENT_HAVE_MORE_THAN_ONE_OUTGOING = "exceptions.process.flow.startEventHaveMoreThanOneOutgoing";
        #endregion

        #region END EVENT EXCEPTIONS        
        public static string FLOW_DIAGRAM_MISSING_END_EVENT = "exceptions.process.flow.missingEndEvent";
        public static string FLOW_DIAGRAM_END_EVENT_MISSING_INCOMING = "exceptions.process.flow.endEventMissingIncoming";
        #endregion

        #region USER TASK EXCEPTIONS
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_TYPE = "exceptions.process.flow.userTaskMissingExecutorType";
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_EXECUTOR_ID = "exceptions.process.flow.userTaskMissingExecutorId";
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_PERSON_ID = "exceptions.process.flow.userTaskMissingPersonId";
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_OPTIONS = "exceptions.process.flow.userTaskMissingOptions";
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_OUTGOING = "exceptions.process.flow.userTaskMissingOutgoing";
        public static string FLOW_DIAGRAM_USER_TASK_MISSING_INCOMING = "exceptions.process.flow.userTaskMissingIncoming";
        public static string FLOW_DIAGRAM_USER_TASK_HAVE_MORE_THAN_ONE_OUTGOING = "exceptions.process.flow.userTaskHaveMoreThanOneOutgoing";
        #endregion

        #region SEND TASK EXCEPTIONS
        public static string FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_TYPE = "exceptions.process.flow.sendTaskMissingDestinataryType";
        public static string FLOW_DIAGRAM_SEND_TASK_MISSING_DESTINATARY_ID = "exceptions.process.flow.sendTaskMissingDestinataryId";
        public static string FLOW_DIAGRAM_SEND_TASK_MISSING_OUTGOING = "exceptions.process.flow.sendTaskMissingOutgoing";
        public static string FLOW_DIAGRAM_SEND_TASK_MISSING_INCOMING = "exceptions.process.flow.sendTaskMissingIncoming";
        public static string FLOW_DIAGRAM_SEND_TASK_HAVE_MORE_THAN_ONE_OUTGOING = "exceptions.process.flow.sendTaskHaveMoreThanOneOutgoing";
        #endregion

        #region EXCLUSIVE GATEWAY EXCEPTIONS
        public static string FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_INCOMING = "exceptions.process.flow.exclusiveGatewayMissingIncoming";
        public static string FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_MISSING_OUTGOING = "exceptions.process.flow.exclusiveGatewayMissingOutgoing";
        public static string FLOW_DIAGRAM_EXCLUSIVE_GATEWAY_OUTGOING_NOT_ASSOCIATED = "exceptions.process.flow.exclusiveGatewayOutgoingNotAssociated";
        #endregion
        #endregion

        #region TASK EXCEPTIONS
        public static string INSERT_TASK_ERROR = "exceptions.task.insertTaskError";
        public static string INSERT_TASK_TRANSACTION_ERROR = "exceptions.task.insertTaskTransactionError";
        public static string UPDATE_TASK_TRANSACTION_ERROR = "exceptions.task.updateTaskTransactionError";
        #endregion

        #region TENANT AUTH EXCEPTIONS
        public static string SUBDOMAIN_DIFFERENT_FROM_INFORMED = "exceptions.task.subdomainDifferentFromInformed";
        public static string ACCESSKEY_DIFFERENT_FROM_INFORMED = "exceptions.task.accessKeyDifferentFromInformed";
        public static string TENANT_ALREADY_INFORMED = "exceptions.task.tenantAlreadyInformed";
        public static string INSERT_TENANTAUTH_ERROR = "exceptions.role.insertTenantAuthError";
        #endregion

        #region S-SIGN INTEGRATION
        public static string MISSING_SSIGN_INTEGRATION_TOKEN = "exceptions.integration.signer.missingIntegrationToken";
        public static string ENVELOPE_ID_SSIGN_NOT_INFORMED = "exceptions.integration.signer.envelopeidssignnotinformed";
        public static string ENVELOPE_SSIGN_NOT_FOUND = "exceptions.integration.signer.envelopessignnotfound";
        public static string ACTION_ENVELOPE_SSIGN_NOT_FOUND = "exceptions.integration.signer.actionenvelopessingnotfound";
        public static string FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_OUTGOING = "exceptions.process.flow.signerIntegrationTaskMissingOutgoing";
        public static string FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_MISSING_INCOMING = "exceptions.process.flow.signerIntegrationTaskMissingIncoming";
        public static string FLOW_DIAGRAM_SSIGN_INTEGRATION_TASK_HAVE_MORE_THAN_ONE_OUTGOING = "exceptions.process.flow.signerIntegrationTaskHaveMoreThanOneOutgoing";
        public static string FILE_FIELD_ASSOCIATED_TO_MORE_THAN_ONE_INTEGRATION_ACTIVITY = "exceptions.integration.signer.fileFieldAssociatedToMoreThanOneIntegrationActivity";
        #endregion
    }
}
