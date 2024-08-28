using System.IO;

namespace SatelittiBpms.Models.ViewModel
{
    public class FileViewModel
    {
        public Stream Content { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
