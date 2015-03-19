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
using Coding4Fun.Toolkit.Controls;
using MyApprovals.Model;
using MyApprovals.Util;
using MyApprovals.Resources;

namespace MyApprovals.Page
{
    public partial class CommentPage : PhoneApplicationPage, IServiceResultHandler
    {
        private Order order;

        public CommentPage()
        {
            InitializeComponent();
        }

         // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowWaiting(false, null);
            order = PhoneApplicationService.Current.State[Constant.KEY_ORDER] as Order;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string message = order.WillReject ? AppResources.RejectingText : AppResources.AddingCommentText;
            ShowWaiting(true, message);
            string comment = tbxComment.Text;
            MagentoService service = new MagentoService(this);
            if (order.WillReject)
                service.Reject(order.Id, comment);
            else
                service.AddComment(order.Id, comment);
        }
        
        void IServiceResultHandler.OnReceiveResult(ServiceResult result)
        {
            ShowWaiting(false, null);
            if (result.ContainsErrorOrFailure())
                MessageBox.Show(result.ToString());
            else
                ShowToastMessage(result.ToString());
        }

        private void ShowWaiting(bool show, string message)
        {
            if (show)
            {
                txbWaiting.Text = message;
                progressOverlay.Show();
            }
            else
                progressOverlay.Hide();
        }

        protected void ShowToastMessage(string message)
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Title = AppResources.ApplicationTitle;
            toast.Message = message;
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.MillisecondsUntilHidden = 2000;
            toast.Completed += Toast_Completed;
            toast.Show();
        }

        void Toast_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            //add some code here
            if (order.WillReject)
            {
                order.NeedReload = false;
                PhoneApplicationService.Current.State[Constant.KEY_ORDER] = order;
                NavigationService.RemoveBackEntry();
                NavigationService.GoBack();
            }
            else
            {
                order.NeedReload = true;
                PhoneApplicationService.Current.State[Constant.KEY_ORDER] = order;
                NavigationService.GoBack();
            }
        }
    }
}