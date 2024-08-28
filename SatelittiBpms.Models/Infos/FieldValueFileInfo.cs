using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class FieldValueFileInfo : BaseInfo
    {
        #region Properties
        public string Key { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public int FieldValueId { get; set; }
        public int UploadedFieldValueId { get; set; }
        public string FileKey { get; set; }
        #endregion

        #region Relationship
        [JsonIgnore]
        public FieldValueInfo FieldValue { get; set; }

        public TaskSignerFileInfo TaskSignerFile { get; set; }

        [JsonIgnore]
        public FieldValueInfo UploadedFieldValue { get; set; }

        public FieldValueFileInfo AsReplicatedNewFieldValueFileInfo(Dictionary<int, TaskSignerInfo> signerTasksCloned)
        {
            return new FieldValueFileInfo()
            {
                Key = Key,
                Size = Size,
                Type = Type,
                Name = Name,
                CreatedDate = CreatedDate,
                CreatedByUserId = CreatedByUserId,
                TenantId = TenantId,
                UploadedFieldValueId = UploadedFieldValueId,
                FieldValueId = FieldValueId,
                FileKey = FileKey,
                TaskSignerFile = TaskSignerFile?.AsReplicatedNewInfo(signerTasksCloned),
            };
        }
        #endregion

        public FieldValueFileViewModel AsFieldValueFileViewModel(IList<SuiteUserViewModel>? userViewModel)
        {
            return new FieldValueFileViewModel()
            {
                CreatedByUserId = CreatedByUserId,
                CreatedDate = CreatedDate,
                Name = Name,
                FileKey = FileKey,
                Size = Size,
                Type = Type,
                NameComponent = FieldValue.Field.Name,                
                UploaderUserName = CreatedByUserId > 0 ? userViewModel?.FirstOrDefault(u => u.Id == CreatedByUserId).Name : "",
                TaskName = UploadedFieldValue?.Task?.Activity?.Name,
                Signed = TaskSignerFile != null && TaskSignerFile.TaskSigner.Status == Enums.TaskSignerStatusEnum.CONCLUDED,
            };
        }
    }
}
