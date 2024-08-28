using SatelittiBpms.Models.Enums;
using System;

namespace SatelittiBpms.Models.Infos
{
    public class ProcessListiningViewModel
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public ProcessStatusEnum Status { get; set; }
        public int ProcessId { get; set; }
        public int ProcessVersionId { get; set; }
        public int? TaskSequance { get; set; }
    }
}