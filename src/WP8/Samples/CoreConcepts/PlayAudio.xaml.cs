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
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace CoreConcepts
{
    public partial class PlayAudio : PhoneApplicationPage
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public PlayAudio()
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
                this.ButtonPlay.IsEnabled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.name = this.NavigationContext.QueryString["name"];
            this.id = this.NavigationContext.QueryString["id"];

            this.TextBlockName.Text = this.name;
        }

        private async void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LiveOperationResult operationResult = await this.liveClient.GetAsync(this.id);
                dynamic properties = operationResult.Result;
                if (properties.source != null)
                {
                    var launcher = new MediaPlayerLauncher()
                    {
                        Media = new Uri(properties.source, UriKind.Absolute),
                        Controls = MediaPlaybackControls.All
                    };

                    launcher.Show();
                }
                else
                {
                    this.ShowError("Could not find the 'source' attribute.");
                }
            }
            catch (LiveConnectException exception)
            {
                this.ShowError(exception.Message);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Format("/SaveFile.xaml?name={0}&id={1}", this.name, this.id);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void ShowError(string message)
        {
            this.NavigationService.Navigate(new Uri("/Error.xaml?msg=" + message, UriKind.Relative));
        }
    }
}