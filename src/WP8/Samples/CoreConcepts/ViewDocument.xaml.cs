using System;
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Live;

namespace CoreConcepts
{
    public partial class ViewDocument : PhoneApplicationPage
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public ViewDocument()
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
                this.ButtonView.IsEnabled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.name = this.NavigationContext.QueryString["name"];
            this.id = this.NavigationContext.QueryString["id"];

            this.TextBlockName.Text = this.name;
        }

        private async void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LiveOperationResult operationResult = await this.liveClient.GetAsync(this.id + "/shared_read_link");
                dynamic properties = operationResult.Result;
                if (properties.link != null)
                {
                    var browser = new WebBrowserTask();

                    browser.Uri = new Uri(properties.link, UriKind.Absolute);
                    browser.Show();
                }
                else
                {
                    this.ShowError("Could not find the 'link' attribute.");
                }
            }
            catch (LiveConnectException exception)
            {
                this.ShowError(exception.Message);
            }
        }

        private void ShowError(string message)
        {
            this.NavigationService.Navigate(new Uri("/Error.xaml?msg=" + message, UriKind.Relative));
        }
    }
}