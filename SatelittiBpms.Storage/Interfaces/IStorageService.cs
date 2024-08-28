using System.IO;
using System.Threading.Tasks;

namespace SatelittiBpms.Storage.Interfaces
{
    public interface IStorageService
    {
        public Task<string> Upload(Stream stream, string folder, string fileName);

        public Task<Stream> Download(string key);

        public Task Delete(string key);
    }
}
