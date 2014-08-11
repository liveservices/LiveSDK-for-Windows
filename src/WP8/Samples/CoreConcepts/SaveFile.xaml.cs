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