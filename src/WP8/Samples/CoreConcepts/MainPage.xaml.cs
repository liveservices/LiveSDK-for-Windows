using System;

using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static LiveConnectSession session;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        public static LiveConnectSession Session
        {
            get
            {
                return MainPage.session;
            }
        }

        private void signInButton_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                MainPage.session = e.Session;
                this.NavigationService.Navigate(new Uri("/Index.xaml", UriKind.Relative));
            }
            else
            {
                MainPage.session = null;
            }
        }
    }
}