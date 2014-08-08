using System;
using System.Windows;
using System.Windows.Controls;

namespace CoreConcepts
{
    public partial class SkyDriveItemTile : UserControl
    {
        public SkyDriveItemTile()
        {
            InitializeComponent();
            this.ButtonItem.Click += ButtonItem_Click;
        }

        public event RoutedEventHandler OnItemSelect;

        public string ItemName
        {
            get
            {
                return this.ButtonItem.Content as string;
            }
            set
            {
                this.ButtonItem.Content = value;
            }
        }

        private void  ButtonItem_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.OnItemSelect;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
