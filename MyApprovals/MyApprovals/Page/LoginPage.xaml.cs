using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyApprovals.Service;
using MyApprovals.Util;
using MyApprovals.Model;
using Coding4Fun.Toolkit.Controls;
using System.Diagnostics;

namespace MyApprovals.Page
{
    public partial class LoginPage : PhoneApplicationPage, IServiceResultHandler
    {
        private MagentoService service;
        public LoginPage()
        {
            InitializeComponent();
            InitControls();
            DisplayStoreURL();
            ShowWaiting(false);
        }

        private void InitControls()
        {
            Session session = Session.GetSavedSession();
            txtStoreURL.Text = session.StoreURL == null ? "" : session.StoreURL;
            txtEmail.Text = session.UserId == null ? "" : session.UserId;
           
            //For test only
            if (false)
            {
                txtStoreURL.Text = Constant.URL_DEFAULT;
                txtEmail.Text = "emrill.fm@36-strategies.com";
                pwbPassword.Password = "123456";
            }

            
            oldLblStoreURLMargin = lblStoreURL.Margin;
            oldTxtStoreURLMargin = txtStoreURL.Margin;
            oldLblEmailMargin = lblEmail.Margin;
            oldTxtEmailMargin = txtEmail.Margin;
            oldLblPasswordMargin = lblPassword.Margin;
            oldPwdPasswordMargin = pwbPassword.Margin;
            oldButtonMargin = btnLogin.Margin;
        }

        Thickness oldLblStoreURLMargin;
        Thickness oldTxtStoreURLMargin;
        Thickness oldLblEmailMargin;
        Thickness oldTxtEmailMargin;
        Thickness oldLblPasswordMargin;
        Thickness oldPwdPasswordMargin;
        Thickness oldButtonMargin;

        void DisplayStoreURL()
        {
            Session session = Session.GetSavedSession();
            txtStoreURL.Text = session.StoreURL == null ? "" : session.StoreURL;
            txtEmail.Text = session.UserId == null ? "" : session.UserId;

            //For test only
            if (false)
            {
                txtStoreURL.Text = Constant.URL_DEFAULT;
                txtEmail.Text = "emrill.fm@36-strategies.com";
                pwbPassword.Password = "123456";
            }

            bool visible = session.WrongLogin || StringUtil.IsNullOrEmptyAfterTrimmed(txtStoreURL.Text);// txtStoreURL.Text == null || txtStoreURL.Text.Trim().Equals("");
            txtStoreURL.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            lblStoreURL.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

            //int delta = 50;
            if (!session.WrongLogin)//Collapse
            {
                lblEmail.Margin = oldLblStoreURLMargin;
                txtEmail.Margin = oldTxtStoreURLMargin;
                lblPassword.Margin = oldLblEmailMargin;
                pwbPassword.Margin = oldTxtEmailMargin;
                btnLogin.Margin = oldLblPasswordMargin;
            }
            else
            {
                lblEmail.Margin = oldLblEmailMargin;
                txtEmail.Margin = oldTxtEmailMargin;
                lblPassword.Margin = oldLblPasswordMargin;
                pwbPassword.Margin = oldPwdPasswordMargin;
                btnLogin.Margin = oldButtonMargin;
            } 
        }

         // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
                this.NavigationService.RemoveBackEntry();
        }

        private void ShowWaiting(bool show)
        {
            if (show)
                progressOverlay.Show();
            else
                progressOverlay.Hide();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = pwbPassword.Password;
            string storeURL = txtStoreURL.Text;
            if (txtStoreURL.Visibility == Visibility.Collapsed)
                storeURL = Session.GetSavedSession().StoreURL;

            if (storeURL == null || storeURL.Trim().Equals(""))
                MessageBox.Show("Store URL must be not empty");
            else
            {
                ShowWaiting(true);
                service = new MagentoService(this);
                service.Login(email, password, storeURL);
            }
        }

        void IServiceResultHandler.OnReceiveResult(ServiceResult result)
        {
            ShowWaiting(false);
            if (result.ContainsErrorOrFailure())
            {
                MessageBoxResult msgResult = MessageBox.Show(result.ToString());
                if (msgResult == MessageBoxResult.OK)
                {
                    DisplayStoreURL();
                }
            }
            else
            {
                if (result.ServiceId == Constant.BIZ_LOGIN)
                {
                    //MessageBox.Show("Login successfully");
                    try
                    {
                        string pushUri = PhoneApplicationService.Current.State[Constant.KEY_PUSH_URI] as string;
                        Debug.WriteLine("Push uri = {0}", pushUri);
                        if(!StringUtil.IsNullOrEmptyAfterTrimmed(pushUri))
                            service.RegisterDevice(pushUri);
                    }
                    catch (KeyNotFoundException e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                   
                    NavigationService.Navigate(new Uri("/Page/MainScreen.xaml", UriKind.Relative));
                  
                }
            }
        }
    }
}