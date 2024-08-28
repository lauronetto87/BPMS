using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{

    public class TaskExecuteViewModel
    {
        public int TaskId { get; set; }

        public string ActivityName { get; set; }

        public int FlowId { get; set; }

        public object FormData { get; set; }

        public int ProcessVersionId { get; set; }

        public string ProcessName { get; set; }

        public string FormContent { get; set; }

        public List<TaskOptionsViewModel> Options { get; set; }
    }

    public class TaskOptionsViewModel
    {
        public int Id { get; set; }

        public string description { get; set; }
    }
}
