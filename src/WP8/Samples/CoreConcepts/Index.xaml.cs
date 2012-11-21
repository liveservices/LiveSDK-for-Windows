using System;
using System.Windows;

using Microsoft.Phone.Controls;

namespace CoreConcepts
{
    public partial class Index : PhoneApplicationPage
    {
        public Index()
        {
            InitializeComponent();
        }

        private void browseSkyDrive_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/BrowseSkyDrive.xaml", UriKind.Relative));
        }

        private void uploadFile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Upload.xaml", UriKind.Relative));
        }
    }
}