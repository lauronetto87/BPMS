using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Storage.Exceptions;
using SatelittiBpms.Storage.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class LocalTempStorageServiceTest
    {
        Mock<IOptions<AwsOptions>> _mockAwsOptions;

        [SetUp]
        public void Setup()
        {
            _mockAwsOptions = new Mock<IOptions<AwsOptions>>();
            _mockAwsOptions.Setup(x => x.Value).Returns(new AwsOptions()
            {
                Storage = new StorageOptions
                {
                    BucketName = "Bucket",
                    FilePath = "Path",
                }
            });
        }

        [Test]
        public async Task ensureUploadDownloadDelete()
        {
            const string fileContent = "Contéudo do arquivo";

            var storage = new LocalTempStorageService(_mockAwsOptions.Object);

            var tempFile = Path.GetTempFileName();
            using (var file = new StreamWriter(tempFile))
            {
                file.Write(fileContent);
                file.Flush();
            }

            string fileKey;

            using (var file = File.OpenRead(tempFile))
            {
                fileKey = await storage.Upload(file, "test", "NameFile.txt");
            }

            Assert.IsTrue(File.Exists(fileKey));

            using (var file = await storage.Download(fileKey))
            {
                using (var reader = new StreamReader(file))
                {
                    Assert.AreEqual(reader.ReadToEnd(), fileContent);
                }
            }

            File.Delete(tempFile);
            await storage.Delete(fileKey);
            Assert.IsFalse(File.Exists(fileKey));
        }


        [Test]
        public void ensureThrows()
        {
            var storage = new LocalTempStorageService(_mockAwsOptions.Object);

            Assert.ThrowsAsync<StorageFileNotExistException>(async () => await storage.Download(Guid.NewGuid().ToString()));

            Assert.ThrowsAsync<StorageFileNotExistException>(async () => await storage.Delete(Guid.NewGuid().ToString()));
        }
    }
}
