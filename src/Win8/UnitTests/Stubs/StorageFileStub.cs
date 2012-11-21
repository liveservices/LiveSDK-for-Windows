namespace Microsoft.Live.Win8.UnitTests.Stubs
{
    using System;

    using Windows.Foundation;
    using Windows.Storage;
    using Windows.Storage.FileProperties;
    using Windows.Storage.Streams;

    class StorageFileStub : IStorageFile
    {
        public IAsyncAction RenameAsync(string desiredName)
        {
            return null;
        }

        public IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            return null;
        }

        public IAsyncAction DeleteAsync()
        {
            return null;
        }

        public IAsyncAction DeleteAsync(StorageDeleteOption option)
        {
            return null;
        }

        public IAsyncOperation<BasicProperties> GetBasicPropertiesAsync()
        {
            return null;
        }

        public bool IsOfType(StorageItemTypes type)
        {
            return true;
        }

        public FileAttributes Attributes { get; private set; }

        public DateTimeOffset DateCreated { get; private set; }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            return null;
        }

        public IAsyncOperation<IInputStream> OpenSequentialReadAsync()
        {
            return null;
        }

        public IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            return null;
        }

        public IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
        {
            return null;
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder)
        {
            return null;
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            return null;
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return null;
        }

        public IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            return null;
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder)
        {
            return null;
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            return null;
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return null;
        }

        public IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            return null;
        }

        public string ContentType { get; private set; }

        public string FileType { get; private set; }
    }
}
