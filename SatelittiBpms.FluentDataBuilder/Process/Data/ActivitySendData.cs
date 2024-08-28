using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivitySendData : ActivityBaseData
    {
        public string Message { get; set; }
        public SendTaskDestinataryTypeEnum DestinataryType { get; internal set; }
        public int? DestinataryId { get; internal set; }
        public string Name { get; internal set; }
        public string TitleMessage { get; internal set; }
        public string CustomEmail { get; internal set; }
    }
}
