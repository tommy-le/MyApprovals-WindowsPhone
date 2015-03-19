using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyApprovals.Resources;
using MyApprovals.Service;
using MyApprovals.Util;
using MyApprovals.Model;
using System.Diagnostics;
using Microsoft.Phone.Notification;
using System.Text;
using MyApprovals.Service;

namespace MyApprovals
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void CheckLogin()
        {
            Session session = Session.GetSavedSession();
            //session.Logout();
            if (session.LoggedIn)
                NavigationService.Navigate(new Uri("/Page/MainScreen.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/Page/LoginPage.xaml", UriKind.Relative));
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            CheckLogin();
        }

        
    }
}