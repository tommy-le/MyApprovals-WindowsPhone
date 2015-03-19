using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyApprovals.Control
{
    public partial class SelectionPopup : UserControl
    {
        public SelectionPopup()
        {
            InitializeComponent();
        }

        public void Show(bool show)
        {
            this.Visibility = Visibility.Visible;
            //if (show)
            //    popSelection.Show();
            //else
            //    popSelection.Hide();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Show(false);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Show(false);
        }
    }
}
