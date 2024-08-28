
using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class ProcessVersionEditViewModel
    {
        public int ProcessVersionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }
        public int? TaskSequance { get; set; }
        public List<ProcessVersionRoleEditViewModel> RolesIds { get; set; }        
        public int ProcessId { get; set; }
        public List<ActivityEditViewModel> ProcessTaskSettingViewModelList { get; internal set; }
        public IList<SignerIntegrationActivityViewModel> SignerTasks { get; set; }
    }
}
