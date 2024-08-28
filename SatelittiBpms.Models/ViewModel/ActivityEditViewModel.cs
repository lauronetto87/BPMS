using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class ActivityEditViewModel
    {
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public IList<FieldEditViewModel> Fields { get; set; }
    }
}
