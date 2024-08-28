using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class FlowGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int>? Ids { get; set; }
    }
}
