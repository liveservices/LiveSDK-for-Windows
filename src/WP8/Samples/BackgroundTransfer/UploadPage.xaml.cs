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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Phone.Controls;

namespace Microsoft.Live.WP8.Samples.BackgroundTransfer
{
    public partial class UploadPage : PhoneApplicationPage, IProgress<LiveOperationProgress>
    {
        private readonly LiveConnectClient connectClient;
        private string currentDirectory;
        private readonly Stack<string> directoryStack;
        private CancellationTokenSource cts;

        public UploadPage()
        {
            InitializeComponent();
            this.connectClient = new LiveConnectClient(((App) App.Current).Session);
            this.directoryStack = new Stack<string>();
            Loaded += OnLoaded;
        }

        public void Report(LiveOperationProgress value)
        {
            this.UploadProgressBar.Value = value.ProgressPercentage;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (this.directoryStack.Count > 0)
            {
                this.currentDirectory = this.directoryStack.Pop();
                this.LoadFileList(this.currentDirectory);

                e.Cancel = true;
            }
        }

        private void PrepareForUpload()
        {
            this.UploadButton.IsEnabled = false;
            this.CancelButton.IsEnabled = true;
            this.cts = new CancellationTokenSource();
            this.UploadProgressBar.Value = 0;
        }

        private void CleanUpAfterUpload()
        {
            this.UploadButton.IsEnabled = true;
            this.CancelButton.IsEnabled = false;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // Look for any previously existing BackgroundUploads.
            // This must be called to clear old requests out of the system.
            foreach (LivePendingUpload pendingUpload in this.connectClient.GetPendingBackgroundUploads())
            {
                this.PrepareForUpload();

                try
                {
                    LiveOperationResult operationResult = await pendingUpload.AttachAsync(this.cts.Token, this);
                    dynamic result = operationResult.Result;
                    MessageBox.Show("Upload successful. Uploaded to " + result.source);
                }
                catch (TaskCanceledException)
                {
                    MessageBox.Show("Upload canceled.");
                }

                this.CleanUpAfterUpload();
            }

            this.currentDirectory = "/";
            this.LoadFileList(this.currentDirectory);
        }

        private void LoadFileList(string path)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var fileItems = new List<FileItem>();

                string searchPattern = Path.Combine(path, "*");
                foreach (string directoryName in store.GetDirectoryNames(searchPattern))
                {
                    fileItems.Add(new FileItem(){Name = directoryName, Type = "directory"});
                }

                foreach (string fileName in store.GetFileNames(searchPattern))
                {
                    fileItems.Add(new FileItem(){Name = fileName, Type="file"});
                }

                this.fileList.ItemsSource = fileItems;
            }
        }

        private void fileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var file = this.fileList.SelectedItem as FileItem;
            if (file == null)
            {
                return;
            }

            switch (file.Type)
            {
                case "directory":
                    this.UploadButton.IsEnabled = false;
                    this.directoryStack.Push(this.currentDirectory);
                    this.currentDirectory = Path.Combine(this.currentDirectory, file.Name);
                    this.LoadFileList(this.currentDirectory);
                    break;

                default:
                    this.UploadButton.IsEnabled = true;
                    break;
            }
        }

        private class FileItem
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private void SetBackgroundTransferPreferences()
        {
            var selectedBackgroundTransferPreference =
                this.BackgroundTransferPreferencesListBox.SelectedItem as ListBoxItem;
            BackgroundTransferPreferences bgTransferPrefs;

            Debug.Assert(Enum.TryParse(selectedBackgroundTransferPreference.Content.ToString(), out bgTransferPrefs));

            this.connectClient.BackgroundTransferPreferences = bgTransferPrefs;
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetBackgroundTransferPreferences();

            var file = this.fileList.SelectedItem as FileItem;
            string path = "me/skydrive";
            var uploadLocation = new Uri(Path.Combine(this.currentDirectory, file.Name), UriKind.RelativeOrAbsolute);
            var overwriteOption = OverwriteOption.Rename;

            this.PrepareForUpload();

            try
            {
                LiveOperationResult operationResult = await this.connectClient.BackgroundUploadAsync(
                    path, 
                    uploadLocation, 
                    overwriteOption,
                    cts.Token,
                    this);

                dynamic result = operationResult.Result;
                MessageBox.Show("Upload successful. Uploaded to " + result.source);
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("Upload canceled.");
            }

            this.CleanUpAfterUpload();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.cts.Cancel();
        }
    }
}