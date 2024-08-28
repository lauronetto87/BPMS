namespace SatelittiBpms.Storage.Exceptions
{
    public class StorageConfigurationNotExistException : BaseException
    {
        private StorageConfigurationNotExistException(string message) : base(message)
        { }

        public static StorageConfigurationNotExistException Create(string property)
        {
            return new StorageConfigurationNotExistException($"Property {property} does not configured.");
        }
    }
}
