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
using MyApprovals.Model;
using MyApprovals.ViewModels;
using MyApprovals.Util;
using System.Diagnostics;
using System.Windows.Data;
using MyApprovals.Resources;
using Microsoft.Phone.Notification;
using System.Text;
using Coding4Fun.Toolkit.Controls;

namespace MyApprovals.Page
{
    public partial class MainScreen : PhoneApplicationPage, IServiceResultHandler
    {
        private int currentPage = Constant.START_PAGE;
        private bool loadingOrders = false;
        //private OrderGroupViewModel viewModel;

        MagentoService service;
        Order order;
        List<OrderGroup> orderGroupList;
        //Order lastOrder;

        public MainScreen()
        {
            InitializeComponent();
            InitApplicationBars();
            service = new MagentoService(this, false);
            order = new Order();
            orderGroupList = new List<OrderGroup>();
            ShowWaiting(false, null);
            //viewModel = new OrderGroupViewModel();
            //DataContext = viewModel;
            //progressOverlay.Hide();

            //this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            //InitializePushNotification();
        }

        //void MainPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var progressIndicator = SystemTray.ProgressIndicator;
        //    if (progressIndicator != null)
        //    {
        //        return;
        //    }

        //    progressIndicator = new ProgressIndicator();

        //    SystemTray.SetProgressIndicator(this, progressIndicator);

        //    Binding binding = new Binding("IsLoading") { Source = viewModel };
        //    BindingOperations.SetBinding(
        //        progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

        //    binding = new Binding("IsLoading") { Source = viewModel };
        //    BindingOperations.SetBinding(
        //        progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

        //    progressIndicator.Text = "Loading more orders...";

        //}
        private void ResetTileNotification()
        {
            var appTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (appTile == null) return; //Don't create...just update
            Uri uri = new Uri("Images/tile_back_reset_bg.png", UriKind.Relative);
            appTile.Update(new StandardTileData() 
            { 
                BackContent = "", 
                Count = 0, 
                Title = AppResources.ApplicationTitle, 
                BackBackgroundImage = uri 
            });
           
        }

        private void ShowWaiting(bool show, string message)
        {
            if (show)
                progressOverlay.Show();
            else     
                progressOverlay.Hide();

            txbWaiting.Text = message;
            llsOrderGroup.IsHitTestVisible = !show;
            EnableApplicationBarButtons(!show);
        }

        void EnableApplicationBarButtons(bool enabled)
        {
            if (ApplicationBar != null)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = enabled;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = enabled;
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (this.NavigationService.BackStack.Any())
                this.NavigationService.RemoveBackEntry();

            DisplaySettings();
            //viewModel.LoadOrders(this.currentPage, Constant.PAGE_SIZE);
            ReloadOrders();
        }

        private void LoadOrders()
        {
            string message = this.currentPage == Constant.START_PAGE ? AppResources.LoadingOrdersText : AppResources.LoadingMoreOrdersText;
            ShowWaiting(true, message);
            loadingOrders = true;
            Debug.WriteLine("Loading orders with page = " + currentPage);
            service.GetMyOrders(Constant.PAGE_SIZE, currentPage);
        }

        public void ReloadOrders()
        {
            orderGroupList.Clear();
            currentPage = Constant.START_PAGE;
            LoadOrders();
        }

        private void LoadMoreOrders()
        {
            currentPage++;
            LoadOrders();
        }

        private void DisplaySettings()
        {
            Session session = Session.GetSavedSession();
            txbEmail.Text = session.UserId;
            tbxStoreURL.Text = session.StoreURL;
        }

        void IServiceResultHandler.OnReceiveResult(ServiceResult result)
        {
            if (result.ContainsErrorOrFailure())
            {
                ShowWaiting(false, null);
                MessageBox.Show(result.ToString());
            }
            else
            {
                if (result.ServiceId == Constant.BIZ_GET_MY_ORDERS)
                {
                    loadingOrders = false;
                    ResetTileNotification();
                    ShowWaiting(false, null);
                    //loadingOrders = false;
                    List<Order> newOrderList = result.GetData() as List<Order>;
                    if (newOrderList == null || newOrderList.Count == 0)
                        currentPage--;
                    else
                    {
                        orderGroupList = OrderGroup.Append(newOrderList, orderGroupList);
                        OrderGroup lastGroup = orderGroupList.Last();
                    }
                    llsOrderGroup.ItemsSource = orderGroupList;
                }
                else if (result.ServiceId == Constant.BIZ_GET_ORDER_DETAIL)
                {
                    Order order = result.GetData() as Order;
                    this.order.Copy(order);
                    this.service.GetApprovers(this.order.Id);
                }
                else if (result.ServiceId == Constant.BIZ_GET_APPROVERS)
                {
                    ShowWaiting(false, null);

                    Order order = result.GetData() as Order;
                    this.order.NextApproverList = order.NextApproverList;
                    this.order.OptionalApproverList = order.OptionalApproverList;
                    this.order.CanGiveFinal = order.CanGiveFinal;

                    PhoneApplicationService.Current.State[Constant.KEY_ORDER] = this.order;
                    NavigationService.Navigate(new Uri("/Page/OrderPage.xaml", UriKind.Relative));
                }
            }
        }

        private void OrderGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowWaiting(true, AppResources.LoadingOrderDetailText);
            this.order = e.AddedItems[0] as Order;
            service.GetOrderDetail(this.order.Id);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Session session = Session.GetSavedSession();
            string storeURL = tbxStoreURL.Text;
            if (storeURL == null || storeURL.Trim().Length == 0)
            {
                MessageBoxResult result = MessageBox.Show(AppResources.StoreURLEmptyText);
                if (result == MessageBoxResult.OK)
                    tbxStoreURL.Text = session.StoreURL;
            }
            else 
                if (!storeURL.Equals(session.StoreURL))
                {
                    session.StoreURL = storeURL;
                    session.Save();
                    Logout();
                }
        }
        
        private void Logout_Click(object sender, EventArgs e)
        {
            Logout();
        }

        private void Logout()
        {
            Session session = Session.GetSavedSession();
            session.Logout();
            NavigationService.Navigate(new Uri("/Page/LoginPage.xaml", UriKind.Relative));

        }

        private void OrderGroup_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            int offset = 1;

            //OrderGroup item = e.Container.Content as OrderGroup;
            //if (item != null && llsOrderGroup.ItemsSource != null)
            //{
               
            //    // Only if there is no data that is currently getting loaded
            //    // would be initiate the loading again
            //    if (!loadingOrders && orderGroupList.Count - orderGroupList.IndexOf(item) <= offset)
            //    {
            //        LoadMoreOrders();
            //    }
            //}

            //if (!loadingOrders && llsOrderGroup.ItemsSource != null && llsOrderGroup.ItemsSource.Count >= offset)
            //{
            //    if (e.ItemKind == LongListSelectorItemKind.Item)
            //    {
            //        Order realizedItem = e.Container.Content as Order;
            //        if (realizedItem != null)
            //        {
            //            if (lastOrder != null && lastOrder.Equals(realizedItem))
            //            {
            //                MessageBox.Show("Reached end item");
            //                LoadMoreOrders();
            //            }
            //            //int index = llsOrderGroup.ItemsSource.IndexOf(realizedItem);
            //            //Debug.WriteLine("Realized item index: " + index);
            //            //if (index >= llsOrderGroup.ItemsSource.Count - offset)
            //            //{
            //            //    LoadMoreOrders();
            //            //}

            //        }
            //    }
            //} 
            //int offsetKnob = 1;
            //if (!viewModel.IsLoading && llsOrderGroup.ItemsSource != null && llsOrderGroup.ItemsSource.Count >= offsetKnob)
            //{
            //    if (e.ItemKind == LongListSelectorItemKind.Item)
            //    {
            //        if ((e.Container.Content as Order).Equals(this.viewModel.LastOrder))
            //        {
            //            this.currentPage++;
            //            this.viewModel.LoadOrders(this.currentPage, Constant.PAGE_SIZE);
            //        }
            //    }
            //}
        }

        private void LoadMore_Click(object sender, EventArgs e)
        {
            LoadMoreOrders();
        }

        private int selectedPivotIndex = 0;

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedPivotIndex = (sender as Pivot).SelectedIndex;
            switch (this.selectedPivotIndex)
            {
                case 0:
                    ApplicationBar = appBarMyApprovals;
                    if (!loadingOrders)
                    {
                        EnableApplicationBarButtons(true);
                        ReloadOrders();
                    }
                    break;
                case 1:
                    ApplicationBar = appBarSettings;
                    EnableApplicationBarButtons(true);
                    break;
                default:
                    break;
            }
        }

        ApplicationBar appBarMyApprovals;
        ApplicationBar appBarSettings;

        void InitApplicationBars()
        {
            appBarMyApprovals = new ApplicationBar();
           
            ApplicationBarIconButton btnLoadMore = new ApplicationBarIconButton(new Uri("/Images/more.png", UriKind.Relative));
            btnLoadMore.Text = AppResources.LoadMoreText;
            btnLoadMore.Click += new EventHandler(LoadMore_Click);
            appBarMyApprovals.Buttons.Add(btnLoadMore);

            ApplicationBarIconButton btnReload = new ApplicationBarIconButton(new Uri("/Images/reload.png", UriKind.Relative));
            btnReload.Text = AppResources.ReloadText;
            btnReload.Click += new EventHandler(Reload_Click);
            appBarMyApprovals.Buttons.Add(btnReload);

            appBarSettings = new ApplicationBar();

            ApplicationBarIconButton btnSave = new ApplicationBarIconButton(new Uri("/Images/save.png", UriKind.Relative));
            btnSave.Text = AppResources.SaveText;
            btnSave.Click += new EventHandler(Save_Click);
            appBarSettings.Buttons.Add(btnSave);

            ApplicationBarIconButton btnLogout = new ApplicationBarIconButton(new Uri("/Images/logout.png", UriKind.Relative));
            btnLogout.Text = AppResources.LogoutText;
            btnLogout.Click += new EventHandler(Logout_Click);
            appBarSettings.Buttons.Add(btnLogout);
        }

        void Reload_Click(object sender, EventArgs e)
        {
            ReloadOrders();
        }


        public void ShowPushToastMessage(string title, string message)
        {
            if (this.selectedPivotIndex !=0) return;
            ToastPrompt toast = new ToastPrompt();
            toast.Title = title;
            toast.Message = message;
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.MillisecondsUntilHidden = 2000;
            toast.Completed += Toast_Push_Completed;
            toast.Show();
        }

        void Toast_Push_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            ReloadOrders();
        }


      
    }
}