using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class ProcessInfo : BaseInfo
    {
        #region Properties
        public int? CurrentVersion { get; set; }
        public int? TaskSequance { get; set; }
        #endregion

        #region Relationships
        public IList<ProcessVersionInfo> ProcessVersions { get; set; }
        #endregion

        public ProcessListiningViewModel AsListingViewModel()
        {
            var currentProcessVersion = ProcessVersions.FirstOrDefault(x => x.Status == ProcessStatusEnum.EDITING);
            if (currentProcessVersion == null)
                currentProcessVersion = ProcessVersions.FirstOrDefault(x => x.Version == CurrentVersion);

            return new ProcessListiningViewModel()
            {
                ProcessId = Id,
                ProcessVersionId = currentProcessVersion.Id,
                Name = currentProcessVersion.Name,
                CreatedDate = currentProcessVersion.CreatedDate,
                CreatedByUserName = currentProcessVersion.CreatedByUserName,
                LastModifiedDate = currentProcessVersion.LastModifiedDate,
                Status = currentProcessVersion.Status,
                TaskSequance = TaskSequance
            };
        }       
    }
}
