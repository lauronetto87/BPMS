
using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.ViewModel
{
    public class FieldEditViewModel
    {
        public string FieldId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldType { get; set; }
        public ProcessTaskFieldStateEnum State { get; set; }
    }
}
