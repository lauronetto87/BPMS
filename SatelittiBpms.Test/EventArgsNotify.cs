using System;

namespace SatelittiBpms.Test
{
    public class EventArgsNotify : EventArgs
    {
        public EventArgsNotify(string connectionId, object message)
        {
            ConnectionId = connectionId;
            Message = message;
        }

        public string ConnectionId;

        public object Message;
    }
}
