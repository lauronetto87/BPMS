using Microsoft.Extensions.Options;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Storage.Exceptions;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SatelittiBpms.Storage.Storage
{
    public class LocalTempStorageService : IStorageService
    {

        private readonly AwsOptions _awsOptions;

        public LocalTempStorageService(IOptions<AwsOptions> awsOptions)
        {
            _awsOptions = awsOptions.Value;
        }

        public async Task<string> Upload(Stream stream, string folder, string fileName)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(_awsOptions?.Storage?.FilePath))
                {
                    throw StorageConfigurationNotExistException.Create("FilePath");
                }
                if (string.IsNullOrEmpty(_awsOptions?.Storage?.BucketName))
                {
                    throw StorageConfigurationNotExistException.Create("BucketName");
                }

                var directory = Path.Combine(Path.GetTempPath(), "IbpmsTempFilesUpload", _awsOptions.Storage.BucketName, _awsOptions.Storage.FilePath, folder);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var key = Guid.NewGuid().ToString().Replace("-", "");

                var filePath = Path.Combine(directory, key + fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var streamNewFile = new FileStream(filePath, FileMode.Create))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(streamNewFile);
                }

                return filePath;
            });
        }

        public async Task<Stream> Download(string key)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(key))
                {
                    throw StorageFileNotExistException.Create(key);
                }
                try
                {
                    return File.OpenRead(key);
                }
                catch (Exception ex)
                {
                    throw StorageGenericException.Create(ex);
                }
            });
        }

        public async Task Delete(string key)
        {
            await Task.Run(() =>
            {
                if (!File.Exists(key))
                {
                    throw StorageFileNotExistException.Create(key);
                }
                try
                {
                    File.Delete(key);
                }
                catch (Exception ex)
                {
                    throw StorageGenericException.Create(ex);
                }
            });
        }

    }
}
