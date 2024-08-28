using Newtonsoft.Json;
using Satelitti.Model;
using System;

namespace SatelittiBpms.Models.Infos
{
    public class TaskHistoryInfo : BaseInfo
    {
        #region Properties
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        #endregion

        #region Relationship
        public int TaskId { get; set; }
        [JsonIgnore]
        public TaskInfo Task { get; set; }

        public int ExecutorId { get; set; }
        [JsonIgnore]
        public UserInfo Executor { get; set; }
        #endregion
    }
}
