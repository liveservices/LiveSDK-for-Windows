// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

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
