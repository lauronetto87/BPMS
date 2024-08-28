using System;

namespace SatelittiBpms.Storage.Exceptions
{
    public class StorageFileNotExistException : BaseException
    {
        private StorageFileNotExistException(string message, Exception innerException) : base(message, innerException)
        { }

        private StorageFileNotExistException(string message) : base(message)
        { }

        public static StorageFileNotExistException Create(string keyOfFile)
        {
            return new StorageFileNotExistException($"`{keyOfFile}` key file not found in storage or not have access.");
        }

        public static StorageFileNotExistException Create(string keyOfFile, Exception innerException)
        {
            return new StorageFileNotExistException($"`{keyOfFile}` key file not found in storage or not have access.", innerException);
        }
    }
}
