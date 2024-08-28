using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivityUserData : ActivityBaseData
    {
        public IList<ActivityFieldData> Fields { get; set; }
        public IList<ButtonData> Buttons { get; set; }
        public UserTaskExecutorTypeEnum ExecutorType { get; internal set; }
        public int? ExecutorId { get; internal set; }
        public int? PersonId { get; internal set; }
    }
}
