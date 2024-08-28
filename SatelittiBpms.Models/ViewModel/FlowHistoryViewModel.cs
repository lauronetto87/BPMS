using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class FlowHistoryViewModel
    {
        public int FlowId { get; set; }
        public string ProcessName { get; set; }
        public string FlowDescription { get; set; }
        public string DiagramContent { get; set; }
        public List<FlowHistoryTaskViewModel> FlowHistoryTasks { get; set; }
    }
}
