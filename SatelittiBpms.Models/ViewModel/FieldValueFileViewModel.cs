using System;

namespace SatelittiBpms.Models.ViewModel
{
    public class FieldValueFileViewModel
    {        
        public long Size { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string NameComponent { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }        
        public string TaskName { get; set; }
        public string UploaderUserName { get; set; }
        public string FileKey { get; set; }
        public bool Signed { get; set; }
    }
}
