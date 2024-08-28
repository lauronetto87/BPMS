using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class FlowQueryViewModel
    {
        public int? Quantity { get; set; }        
        public List<FlowViewModel> List { get; set; }        
    }
}
