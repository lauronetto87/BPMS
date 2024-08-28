using System.IO;

namespace SatelittiBpms.Models.DTO
{
    public class FileToFieldValueDTO
    {
        public int TaskId { get; set; }
        public string ComponentInternalId { get; set; }
        public string FileContentType { get; set; }
        public string FileName { get; set; }
        public Stream Stream { get; set; }
    }
}