namespace SatelittiBpms.Models.Constants
{
    public class XmlDiagramConstants
    {
        public const string BPMN2_NAMESPACE_PREFIX = "bpmn2";
        public const string BPMN2_NAMESPACE_URI = "http://www.omg.org/spec/BPMN/20100524/MODEL";
        public const string SATELITTI_NAMESPACE_PREFIX = "satelitti";
        public const string SATELITTI_NAMESPACE_URI = "http://selbetti/schema/bpmn/satelitti";
        public const string EXECUTOR_TYPE_ATTRIBUTE = "executorType";
        public const string EXECUTOR_ID_ATTRIBUTE = "executorId";
        public const string PERSON_ID_ATTRIBUTE = "personId";
        public const string TASK_OPTIONS_NODE_NAME = "taskOptions";
        public const string TASK_OPTION_NODE_NAME = "taskOption";
        public const string DESTINATARY_TYPE_ATTRIBUTE = "destinataryType";
        public const string DESTINATARY_ID_ATTRIBUTE = "destinataryId";
        public const string CUSTOM_EMAIL_ID_ATTRIBUTE = "customEmail";
        public const string OUTGOING_NODE_NAME = "outgoing";
        public const string INCOMING_NODE_NAME = "incoming";
        public const string MESSAGE_NOTIFICATION = "message";
        public const string TITLE_MESSAGE_NOTIFICATION = "titleMessage";
        public const string SEQUENCE_FLOW_NODE_NAME = "sequenceFlow";
        public const string OPTION_ATTRIBUTE = "option";
        public const string START_EVENT_ACTIVITY = "startEvent";
        public const string END_EVENT_ACTIVITY = "endEvent";
        public const string SEND_TASK_ACTIVITY = "sendTask";
        public const string USER_TASK_ACTIVITY = "userTask";
        public const string EXCLUSIVE_GATEWAY_ACTIVITY = "exclusiveGateway";
        public const string SIGNER_INTEGRATION_ACTIVITY = "satelittiSigner";
        public const string LANESET = "laneSet";
        public const string SIGNER_TASK = "satelittiSigner";
    }
}