using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApprovals.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MyApprovals.Model
{
    [DataContract]
    public class OrderItem : INotifyPropertyChanged
    {
        private string name;
        private string subTotal;
        private string uom;
        private string qty;

         [DataMember]
        public string Name {
            get 
            {
                return this.name; 
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

         [DataMember]
        public string SubTotal
        {
            get
            {
                return this.subTotal;
            }

            set
            {
                if (value != this.subTotal)
                {
                    this.subTotal = value;
                    NotifyPropertyChanged("SubTotal");
                }
            }
        }

         [DataMember]
        public string Uom
        {
            get
            {
                return this.uom;
            }

            set
            {
                if (value != this.uom)
                {
                    this.uom = value;
                    NotifyPropertyChanged("Uom");
                }
            }
        }

         [DataMember]
        public string Qty
        {
            get
            {
                return this.qty;
            }

            set
            {
                if (value != this.qty)
                {
                    this.qty = value;
                    NotifyPropertyChanged("Qty");
                }
            }
        }

        public OrderItem()
        {
            // TODO Auto-generated constructor stub
        }

        public static OrderItem fromJObject(JObject jObject)
        {
            try
            {
                OrderItem orderItem = new OrderItem();
                orderItem.Uom = (string)jObject[Constant.KEY_UOM];
                orderItem.SubTotal = (string)jObject[Constant.KEY_SUB_TOTAL];
                orderItem.Name = (string)jObject[Constant.KEY_NAME];
                orderItem.Qty = (string)jObject[Constant.KEY_QTY];
                return orderItem;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        public static List<OrderItem> fromJArray(JArray jArray)
        {
            List<OrderItem> dataList = new List<OrderItem>();
            if (jArray == null) return dataList;

            foreach (JObject jObject in jArray)
            {
                OrderItem data = fromJObject(jObject);
                if(data != null)
                    dataList.Add(data);
            }

            return dataList;
        }

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
