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
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Microsoft.Live;
using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class Download : PhoneApplicationPage, IProgress<LiveOperationProgress>
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public Download()
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
                this.ButtonDownload.IsEnabled = true;
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

        private async void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            this.ShowProgress();
            try
            {
                LiveDownloadOperationResult downloadOperationResult =
                    await this.liveClient.DownloadAsync(this.id + "/content", new CancellationToken(false), this);
                Stream fileContent = downloadOperationResult.Stream;
                if (IsImageFile(this.name))
                {
                    this.ShowImage(fileContent);
                }
                else
                {
                    this.ShowText(fileContent);
                }
            }
            catch (LiveConnectException exception)
            {
                this.ShowMessage(exception.Message);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Format("/SaveFile.xaml?name={0}&id={1}", this.name, this.id);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void ShowImage(Stream file)
        {
            using (file)
            {
                var imgSource = new BitmapImage();
                imgSource.SetSource(file);
                this.Image.Source = imgSource;
            }

            this.TextBox.Visibility = Visibility.Collapsed;
            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.Image.Visibility = Visibility.Visible;
        }

        private void ShowProgress()
        {
            this.TextBox.Visibility = Visibility.Collapsed;
            this.Image.Visibility = Visibility.Collapsed;

            this.ProgressBar.Visibility = Visibility.Visible;
        }

        private void ShowText(Stream file)
        {
            using (var sr = new StreamReader(file))
            {
                this.TextBox.Text = sr.ReadToEnd();
            }

            this.Image.Visibility = Visibility.Collapsed;
            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.TextBox.Visibility = Visibility.Visible;
        }

        private void ShowMessage(string message)
        {
            this.Image.Visibility = Visibility.Collapsed;

            this.TextBox.Text = message;
            this.TextBox.Visibility = Visibility.Visible;
        }

        private static bool IsImageFile(string fileName)
        {
            string[] segments = fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string extention = segments[segments.Length - 1];

            switch (extention.ToLowerInvariant())
            {
                case "jpg":
                case "png":
                case "ico":
                case "jif":
                case "gif":
                case "jpeg":
                case "bmp":
                case "tif":
                case "tiff":
                    return true;

                default:
                    return false;
            }
        }
    }
}