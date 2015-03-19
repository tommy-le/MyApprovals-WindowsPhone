using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Controls;

namespace MyApprovals.Page
{
    public partial class BasePage : PhoneApplicationPage
    {
        public BasePage()
        {
            InitializeComponent();
        }

        protected void ShowToastMessage(string message)
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Title = "My Approvals";
            toast.Message = message;
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.MillisecondsUntilHidden = 2000;
            toast.Show();
        }

        protected void ShowDialogMessage(string message)
        {
            MessageBox.Show(message, "My Approvals", MessageBoxButton.OK);
        }

        protected MessageBoxResult ShowConfirmMessage(string message)
        {
            return MessageBox.Show(message, "My Approvals", MessageBoxButton.OKCancel);
        }
    }
}