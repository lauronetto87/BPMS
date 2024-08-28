using SatelittiBpms.Models.Enums;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class ProcessListingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedByUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public ProcessStatusEnum Status { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }

        public IList<ProcessVersionRoleEditViewModel> RolesIds { get; set; }
        public IList<ActivityEditViewModel> ProcessTaskSettingViewModelList { get; set; }
    }
}
