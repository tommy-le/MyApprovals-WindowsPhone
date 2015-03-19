using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MyApprovals.Util;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Globalization;

namespace MyApprovals.Model
{
    [DataContract]
    public class Order : IComparable, INotifyPropertyChanged
    {
        public Order()
        {
            this.ActionList = new List<Action>();
            this.ItemList = new List<OrderItem>();
            this.NextApproverList = new List<Approver>();
            this.OptionalApproverList = new List<Approver>();
            this.WillReject = false;
            this.NeedReload = false;
        }

        public void Copy(Order order)
        {
            this.CreatedDate = order.CreatedDate;
            this.OrderedBy = order.OrderedBy;

            if (order.ActionList != null && order.ActionList.Count() > 0)
                this.ActionList = order.ActionList;

            if (order.ItemList != null && order.ItemList.Count() > 0)
                this.ItemList = order.ItemList;

            if (order.NextApproverList != null && order.NextApproverList.Count() > 0)
                this.NextApproverList = order.NextApproverList;

            if (order.OptionalApproverList != null && order.OptionalApproverList.Count() > 0)
                this.OptionalApproverList = order.OptionalApproverList;

        }

        int IComparable.CompareTo(object obj)
        {
            Order order = obj as Order;
            string timeFormat = Constant.FORMAT_TIME_AMPM;//"h:mm tt";
            DateTime myCreatedTime = DateTime.ParseExact(this.CreatedTime, timeFormat, CultureInfo.InvariantCulture);
            DateTime orderCreatedTime = DateTime.ParseExact(order.CreatedTime, timeFormat, CultureInfo.InvariantCulture);
            //DateTime myCreatedTime = DateTime.ParseExact(this.CreatedTime, timeFormat, null);
            //DateTime orderCreatedTime = DateTime.ParseExact(order.CreatedTime, timeFormat, null);
           
            return orderCreatedTime.CompareTo(myCreatedTime);
        }

        /*
	 {\"result_code\":1,
	 \"SID\":\"camh8o73341tq5jjig04s00r36\",
	 \"orders\":
	 	[{\"id\":\"1285\",
	 	\"increment_id\":\"200000181\",\"contract_id\":\"3\",\"contract_name\":\"Emril test contract\",\"created_at\":\"2014-01-16 04:43:18\",\"updated_at\":\"2014-01-16 10:49:22\",\"grouped_by_date\":\"January 16, 2014\",\"created_time\":\"4:43 AM\",\"total\":\"AED22.00\",\"status\":\"Pending Approval\"},{\"id\":\"628\",\"increment_id\":\"100000513\",\"contract_id\":\"1\",\"contract_name\":\"11925 - Downtown\",\"created_at\":\"2013-07-22 05:10:24\",\"updated_at\":\"2014-01-16 10:58:43\",\"grouped_by_date\":\"July 22, 2013\",\"created_time\":\"5:10 AM\",\"total\":\"AED100,001.00\",\"status\":\"Pending Approval\"}]}

	 
	 {\"result_code\":0,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}
	 */
        public static Order Parse(JObject jObject)
        {
            try
            {
                JArray actionArray = (JArray)jObject[Constant.KEY_COMMENTS];
                JArray itemArray = (JArray)jObject[Constant.KEY_ITEMS];
                Order order = new Order();
                order.Id = (string)jObject[Constant.KEY_ID];
                order.IncrementId = "#" + (string)jObject[Constant.KEY_INCREMENT_ID];
                order.Contract = (string)jObject[Constant.KEY_CONTRACT_NAME];
                if(order.Contract == null || order.Contract.Trim().Equals(""))
                    order.Contract = (string)jObject[Constant.KEY_CONTRACT];
                order.Amount = (string)jObject[Constant.KEY_TOTAL];
                order.OrderedBy = (string)jObject[Constant.KEY_ORDERED_BY];
                order.GroupedBy = (string)jObject[Constant.KEY_GROUPED_BY_DATE];
                order.CreatedAt = (string)jObject[Constant.KEY_CREATED_AT];
                order.CreatedDate = (string)jObject[Constant.KEY_CREATED_DATE];
                order.CreatedTime = (string)jObject[Constant.KEY_CREATED_TIME];
                order.ActionList = (List<Action>)Action.fromJArray(actionArray);
                order.ItemList = (List<OrderItem>)OrderItem.fromJArray(itemArray);
                return order;
              
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        public static List<Order> Parse(JArray jArray)
        {
            List<Order> dataList = new List<Order>();
            if (jArray == null) return dataList;

            foreach (JObject jObject in jArray)
            {
                Order data = Order.Parse(jObject);
                if(data != null)
                    dataList.Add(data);
            }

            return dataList;
        }

         [DataMember]
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

         [DataMember]
        public string Contract
        {
            get
            {
                return this.contract;
            }

            set
            {
                if (value != this.contract)
                {
                    this.contract = value;
                    NotifyPropertyChanged("Contract");
                }
            }
        }

         [DataMember]
        public string GroupedBy
        {
            get
            {
                return this.groupedBy;
            }

            set
            {
                if (value != this.groupedBy)
                {
                    this.groupedBy = value;
                    NotifyPropertyChanged("GroupedBy");
                }
            }
        }

         [DataMember]
        public string OrderedBy
        {
            get
            {
                return this.orderedBy;
            }

            set
            {
                if (value != this.orderedBy)
                {
                    this.orderedBy = value;
                    NotifyPropertyChanged("OrderedBy");
                }
            }
        }

         [DataMember]
        public string Amount
        {
            get
            {
                return this.amount;
            }

            set
            {
                if (value != this.amount)
                {
                    this.amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

         [DataMember]
        public string CreatedDate
        {
            get
            {
                return this.createdDate;
            }

            set
            {
                if (value != this.createdDate)
                {
                    this.createdDate = value;
                    NotifyPropertyChanged("CreatedDate");
                }
            }
        }

         [DataMember]
        public string CreatedTime
        {
            get
            {
                return this.createdTime;
            }

            set
            {
                if (value != this.createdTime)
                {
                    this.createdTime = value;
                    NotifyPropertyChanged("CreatedTime");
                }
            }
        }

         [DataMember]
        public string CreatedAt
        {
            get
            {
                return this.createdAt;
            }

            set
            {
                if (value != this.createdAt)
                {
                    this.createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

         [DataMember]
        public string IncrementId
        {
            get
            {
                return this.incrementId;
            }

            set
            {
                if (value != this.incrementId)
                {
                    this.incrementId = value;
                    NotifyPropertyChanged("IncrementId");
                }
            }
        }

         [DataMember]
        public bool CanGiveFinal
        {
            get
            {
                return this.canGiveFinal;
            }

            set
            {
                if (value != this.canGiveFinal)
                {
                    this.canGiveFinal = value;
                    NotifyPropertyChanged("CanGiveFinal");
                }
            }
        }

         [DataMember]
        public bool NeedReload { get; set; }
         [DataMember]
        public bool WillReject { get; set; }

        private string id;
        private string contract;
        private string groupedBy;
        private string orderedBy;
        private string amount;
        private string createdDate;
        private string createdTime;
        private string createdAt;
        private string incrementId;
        private bool canGiveFinal;
      
         [DataMember]
        public List<Approver> NextApproverList { get; set; }
         [DataMember]
        public List<Approver> OptionalApproverList { get; set; }
         [DataMember]
        public List<OrderItem> ItemList { get; set; }
         [DataMember]
        public List<Action> ActionList { get; set; }
         [DataMember]
        public List<OrderItem> OrderItemList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
