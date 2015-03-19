using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyApprovals.Model;
using MyApprovals.Util;

namespace MyApprovals.Page
{
    public partial class OrderItemsPage : PhoneApplicationPage
    {
        public OrderItemsPage()
        {
            InitializeComponent();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Order order = PhoneApplicationService.Current.State[Constant.KEY_ORDER] as Order;
            DataContext = order;
            llsOrderItems.ItemsSource = order.ItemList;
        }
    }
}