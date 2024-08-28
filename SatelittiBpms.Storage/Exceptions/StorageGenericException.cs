using System;

namespace SatelittiBpms.Storage.Exceptions
{
    public class StorageGenericException : BaseException
    {
        private StorageGenericException(string message, Exception innerException) : base(message, innerException)
        { }

        private StorageGenericException(string message) : base(message)
        { }

        public static StorageGenericException Create(Exception innerException)
        {
            return new StorageGenericException("A generic error occurred with storage. Check the Stack for more information.", innerException);
        }

        public static StorageGenericException Create(string message)
        {
            return new StorageGenericException(message);
        }
    }
}
