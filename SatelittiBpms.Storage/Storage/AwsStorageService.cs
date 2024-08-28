using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Storage.Exceptions;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace SatelittiBpms.Storage.Storage
{
    [ExcludeFromCodeCoverage]
    public class AwsStorageService : IStorageService
    {
        private readonly AwsOptions _awsOptions;

        public AwsStorageService(IOptions<AwsOptions> awsOptions)
        {
            _awsOptions = awsOptions.Value;
        }

        public async Task<string> Upload(Stream stream, string folder, string fileName)
        {
            if (string.IsNullOrEmpty(_awsOptions?.Storage?.FilePath))
            {
                throw StorageConfigurationNotExistException.Create(nameof(_awsOptions.Storage));
            }
            if (string.IsNullOrEmpty(_awsOptions?.Storage?.BucketName))
            {
                throw StorageConfigurationNotExistException.Create(nameof(_awsOptions.Storage));
            }

            var key = Guid.NewGuid().ToString().Replace("-", "");
            key = Path.Combine(_awsOptions.Storage.FilePath, folder, key + fileName);

            try
            {
                var fileTransferUtility = new TransferUtility(CreateClientInstance());
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _awsOptions.Storage.BucketName,
                    AutoCloseStream = false,
                    InputStream = stream,
                    Key = key,
                };
                await fileTransferUtility.UploadAsync(request);

                return key;
            }
            catch (AmazonS3Exception ex)
            {
                throw StorageGenericException.Create(ex);
            }
            catch (Exception ex)
            {
                throw StorageGenericException.Create(ex);
            }            
        }

        public async Task<Stream> Download(string key)
        {
            if (string.IsNullOrEmpty(_awsOptions?.Storage?.BucketName))
            {
                throw StorageConfigurationNotExistException.Create(nameof(_awsOptions.Storage));
            }

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _awsOptions.Storage.BucketName,
                    Key = key
                };
                var objectResult = await CreateClientInstance().GetObjectAsync(request);
                return objectResult.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw StorageFileNotExistException.Create(key, ex);
                }
                throw StorageGenericException.Create(ex);
            }
            catch (Exception ex)
            {
                throw StorageGenericException.Create(ex);
            }
        }


        public async Task Delete(string key)
        {
            if (string.IsNullOrEmpty(_awsOptions?.Storage?.BucketName))
            {
                throw StorageConfigurationNotExistException.Create(nameof(_awsOptions.Storage));
            }

            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _awsOptions.Storage.BucketName,
                    Key = key
                };
                await CreateClientInstance().DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw StorageFileNotExistException.Create(key, ex);
                }
                throw StorageGenericException.Create(ex);
            }
            catch (Exception ex)
            {
                throw StorageGenericException.Create(ex);
            }
        }


        protected virtual IAmazonS3 CreateClientInstance()
        {
            return new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
        }
    }
}
