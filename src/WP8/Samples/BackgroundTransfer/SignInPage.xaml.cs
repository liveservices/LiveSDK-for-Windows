using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;

using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Microsoft.Live.WP8.Samples.BackgroundTransfer
{
    /// <summary>
    /// Sign in page uses the Live SDK Sign In Control to perform the authentication.
    /// Once authenticated it places the retrieved LiveConnectSession in the global Application
    /// class to be used by all the pages.
    /// </summary>
    public partial class SignInPage : PhoneApplicationPage
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void OnSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                ((App) App.Current).Session = e.Session;
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}