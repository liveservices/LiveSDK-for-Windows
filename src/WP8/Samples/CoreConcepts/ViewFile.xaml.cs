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