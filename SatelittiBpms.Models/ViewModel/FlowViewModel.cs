using SatelittiBpms.Models.Enums;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class FlowViewModel
    {
        public int Id { get; set; }
        public int FlowId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public int? ExecutorId { get; set; }
        public string ExecutorName { get; set; }
        public int ActivityId { get; set; }
        public int ProcessVersionId { get; set; }
        public string ProcessVersionName { get; set; }
        public bool Finished { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedByUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public string CurrentRoleName { get; set; }        
        public string ActivityName { get; set; }
        public ProcessStatusEnum ProcessStatus { get; set; }
        public UserTaskExecutorTypeEnum? ExecutorType { get; set; }
        public bool? isMyRole { get; set; }

        public FlowGroupViewModel AsActivitieListingViewModel()
        {
            return new FlowGroupViewModel()
            {
                Id = ActivityId,
                Name = ActivityName,
                Description = ProcessVersionName,
                Ids = new List<int>()
            };
        }

        public FlowGroupViewModel AsProcessVersionListingViewModel()
        {
            return new FlowGroupViewModel()
            {
                Id = ProcessVersionId,
                Name = Name,
                Ids = new List<int>()
            };
        }
    }
}
