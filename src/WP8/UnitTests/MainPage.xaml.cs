namespace Microsoft.Live.WP8.UnitTests
{
    using System;
    using System.Windows;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Silverlight.Testing;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.IsVisible = false;

            var testPage = UnitTestSystem.CreateTestPage();

            // TODO(skrueger): See if we can get an updated Microsoft.Silverlight.Testing.dll that supports WP8.
            // On WP8, testPage is NOT an IMobileTestPage. This seems to mean that the current
            // Microsoft.Silverlight.Testing.dll does not support WP8. See if
            var mobilePage = testPage as IMobileTestPage;
            BackKeyPress += (x, xe) => xe.Cancel = mobilePage.NavigateBack();
            (Application.Current.RootVisual as PhoneApplicationFrame).Content = testPage;
        }
    }
}