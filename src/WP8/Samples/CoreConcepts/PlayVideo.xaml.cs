using System;
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Live;
using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class PlayVideo : PhoneApplicationPage
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public PlayVideo()
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
        }

        private async void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LiveOperationResult operationResult = await this.liveClient.GetAsync(this.id);
                dynamic properties = operationResult.Result;
                if (properties.source != null)
                {
                    this.MainMedia.Source = new Uri(properties.source, UriKind.Absolute);
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