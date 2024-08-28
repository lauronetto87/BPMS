namespace SatelittiBpms.Workflow.Models
{
    public class EventUserInfo
    {
        private const string EVENT_NAME = "eventName";
        private const string EVENT_KEY = "eventKey";

        public EventUserInfo(int taskId)
        {
            eventKey = EVENT_KEY + taskId;
            eventName = EVENT_NAME;
        }
        
        public string eventKey { get; }
        public string eventName { get; }
        
    }
}
