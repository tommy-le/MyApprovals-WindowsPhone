using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using MyApprovals.Model;
using MyApprovals.Util;
using MyApprovals.Service;
using MyApprovals.Control;
using System.Windows.Media;
using MyApprovals.Resources;
using Coding4Fun.Toolkit.Controls;

namespace MyApprovals.Page
{
    public partial class OrderPage : PhoneApplicationPage, IServiceResultHandler
    {
        private Approver selectedApprover;
        private MagentoService service;
        private Order order;
        private static int NEXT = 1;
        private static int OPT = 2;
        private static int NONE = 3;
        private int operation = OPT;
        
        public OrderPage()
        {
            InitializeComponent();
            service = new MagentoService(this);
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("xxxx Display again");
            ShowWaiting(false, null);
            
            order = PhoneApplicationService.Current.State[Constant.KEY_ORDER] as Order;
            order.WillReject = false;
            DisplayOrder(order);

            if (order.NeedReload)
            {
                ShowWaiting(true, AppResources.UpdatingOrderText);
                service.GetOrderDetail(order.Id);
            }
            
        }

        private void DisplayOrder(Order order)
        {
            DataContext = order;
           
            llsAction.ItemsSource = order.ActionList;

            if (order.NextApproverList == null || order.NextApproverList.Count == 0)
                btnNext.IsEnabled = false;
            else
                btnNext.IsEnabled = true;

            if (order.OptionalApproverList == null || order.OptionalApproverList.Count == 0)
                btnOptional.IsEnabled = false;
            else
                btnOptional.IsEnabled = true;
                
            if (order.CanGiveFinal)
            {
                btnNext.Content = AppResources.FinalApproveText;
                btnNext.IsEnabled = true;
            }
        }

        int serviceId = Constant.BIZ_NONE;

        void IServiceResultHandler.OnReceiveResult(ServiceResult result)
        {
            ShowWaiting(false, null);
            serviceId = result.ServiceId;
            if (result.ContainsErrorOrFailure())
                MessageBox.Show(result.ToString());
            else 
            {
                switch (result.ServiceId)
                {
                    case Constant.BIZ_GET_ORDER_DETAIL:
                        Order order = (Order)result.GetData();
                        order.NextApproverList = this.order.NextApproverList;
                        order.OptionalApproverList = this.order.OptionalApproverList;
                        order.CanGiveFinal = this.order.CanGiveFinal;
                        this.order = order;
                        DisplayOrder(this.order);
                        break;
                    case Constant.BIZ_APPROVE_NEXT:
                    case Constant.BIZ_APPROVE_OPTIONAL:
                        if (result.ContainsErrorOrFailure())
                            MessageBox.Show(result.ToString());
                        else
                            ShowToastMessage(result.ToString());
                        break;
                }
            }
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
            NavigationService.GoBack();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(order.CanGiveFinal)
            {
                MessageBoxResult result = MessageBox.Show(AppResources.ConfirmApproveText, AppResources.ApplicationTitle, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    ShowWaiting(true, AppResources.ApprovingText);
                    service.NextApprover(order.Id, "");
                }
            }
            else 
            {
                llsApprovers.ItemsSource = order.NextApproverList;
                operation = NEXT;
                popApprovers.Show();
            }
        }

        private void Optional_Click(object sender, RoutedEventArgs e)
        {
            llsApprovers.ItemsSource = order.OptionalApproverList;
            operation = OPT;
            popApprovers.Show();
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
             MessageBoxResult result = MessageBox.Show(AppResources.ConfirmRejectText, AppResources.ApplicationTitle, MessageBoxButton.OKCancel);
             if (result == MessageBoxResult.OK)
             {
                 order.WillReject = true;
                 PhoneApplicationService.Current.State[Constant.KEY_ORDER] = order;
                 NavigationService.Navigate(new Uri("/Page/CommentPage.xaml", UriKind.Relative));
             }
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            order.WillReject = false;
            order.NeedReload = false;
            PhoneApplicationService.Current.State[Constant.KEY_ORDER] = order;
            
            NavigationService.Navigate(new Uri("/Page/OrderItemsPage.xaml", UriKind.Relative));
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedApprover != null)
            {
                popApprovers.Hide();
                MessageBoxResult result = MessageBox.Show(AppResources.ConfirmApproveText, AppResources.ApplicationTitle, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Debug.WriteLine("Select approver: " + selectedApprover.Name);
                    if (operation == NEXT)
                    {
                        ShowWaiting(true, AppResources.ApprovingText);
                        service.NextApprover(order.Id, selectedApprover.Id);
                    }
                    else
                        if (operation == OPT)
                        {
                            ShowWaiting(true, AppResources.ApprovingText);
                            service.OptionalApprover(order.Id, selectedApprover.Id);
                        }

                    operation = NONE;
                   
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            popApprovers.Hide();
            operation = NONE;
        }

        private void Approvers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedApprover = llsApprovers.SelectedItem as Approver;
            LongListSelectorUtil.HighlightSelectionItem(llsApprovers, e);
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

        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            order.WillReject = false;
            NavigationService.Navigate(new Uri("/Page/CommentPage.xaml", UriKind.Relative));
        }
    }
}