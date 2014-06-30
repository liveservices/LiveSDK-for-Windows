// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Live;
using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class SaveFile : PhoneApplicationPage, IProgress<LiveOperationProgress>
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public SaveFile()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Loaded -= this.OnLoaded;

            if (MainPage.Session == null)
            {
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                this.liveClient = new LiveConnectClient(MainPage.Session);
                this.ButtonSave.IsEnabled = true;
            }
        }

        public void Report(LiveOperationProgress value)
        {
            this.ProgressBar.Value = value.ProgressPercentage;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.name = this.NavigationContext.QueryString["name"];
            this.id = this.NavigationContext.QueryString["id"];

            this.TextBlockName.Text = this.name;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LiveDownloadOperationResult downloadOperationResult =
                    await this.liveClient.DownloadAsync(this.id + "/content", new CancellationToken(false), this);
                this.SaveToStorage(downloadOperationResult.Stream);
            }
            catch (LiveConnectException exception)
            {
                this.TextBox.Text = exception.Message;
            }
        }

        private void SaveToStorage(Stream file)
        {
            using (IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var isoFileStream = new IsolatedStorageFileStream(this.name, FileMode.OpenOrCreate, myStore))
                {
                    using (file)
                    {
                        long length = file.Length;
                        byte[] buffer = new byte[length];

                        int readLength = file.Read(buffer, 0, (int)length);

                        isoFileStream.Write(buffer, 0, readLength);
                    }
                }

                try
                {
                    // Specify the file path and options.
                    using (var isoFileStream = new IsolatedStorageFileStream(this.name, FileMode.Open, myStore))
                    {
                        this.TextBox.Text = "File stored in the isolated storage: \n" + this.name;
                    }
                }
                catch
                {
                    this.TextBox.Text = "File not found: \n" + this.name;
                }
            }
        }
    }
}