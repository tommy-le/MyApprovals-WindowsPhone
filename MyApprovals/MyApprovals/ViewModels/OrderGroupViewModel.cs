using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MyApprovals.Model;
using MyApprovals.Service;
using MyApprovals.Util;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace MyApprovals.ViewModels
{
    class OrderGroupViewModel : INotifyPropertyChanged, IServiceResultHandler
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool isLoading = false;

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }

        private MagentoService service;
        private bool hasNoMore = false;

        public OrderGroupViewModel()
        {
            this.orderGroupList = new List<OrderGroup>();
            this.OrderGroupCollection = new ObservableCollection<OrderGroup>();
            this.service = new MagentoService(this);
            this.IsLoading = false;
        }

        //public OrderGroupViewModel(List<OrderGroup> orderGroupList)
        //{
        //    this.service = new MagentoService(this);
        //    if(orderGroupList != null)
        //        this.OrderGroupList = new ObservableCollection<OrderGroup>(orderGroupList);
        //    else
        //        this.OrderGroupList = new ObservableCollection<OrderGroup>();
        //}
        
        private ObservableCollection<OrderGroup> orderGroupCollection;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<OrderGroup> OrderGroupCollection
        {
            get
            {
                return orderGroupCollection;
            }
            set
            {
                orderGroupCollection = value;
                NotifyPropertyChanged("OrderGroupCollection");

            }
        }

        public Order LastOrder { get; set; }
        private List<OrderGroup> orderGroupList;

        public void LoadOrders(int pageIndex, int pageSize)
        {
            if (this.hasNoMore) return;
            Debug.WriteLine("Loading orders with pageindex = {0}, pagesize={0}", pageIndex, pageSize);
            if (pageIndex == Constant.START_PAGE)
                this.orderGroupList.Clear();
            this.IsLoading = true;
            this.service.GetMyOrders(pageSize, pageIndex);
        }

        void IServiceResultHandler.OnReceiveResult(ServiceResult result)
        {
            if (result.ContainsErrorOrFailure())
            {
                this.IsLoading = false;
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                MessageBox.Show(result.ToString());
                //});
            }
            else
            {
                List<Order> newOrderList = result.GetData() as List<Order>;
                if (newOrderList != null && newOrderList.Count > 0)
                {
                    this.hasNoMore = false;
                    this.orderGroupList = OrderGroup.Append(newOrderList, this.orderGroupList);
                    OrderGroup lastGroup = this.orderGroupList.Last();
                    this.LastOrder = lastGroup.Last();
                    this.OrderGroupCollection = new ObservableCollection<OrderGroup>(this.orderGroupList);
                }
                else this.hasNoMore = true;

                this.IsLoading = false;
            
            }
        }
    }
}
