using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MyApprovals.Util;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MyApprovals.Model
{

    [DataContract]
    public class OrderGroup : List<Order>, IComparable
    {
        public OrderGroup()
        {
		}

         [DataMember]
	    public string Title { get; set;}
         [DataMember]
	    public int GroupIndex { get;set;}
         [DataMember]
        public string SortValue { get; set; }

        int IComparable.CompareTo(object obj)
        {
            OrderGroup orderGroup = obj as OrderGroup;
            //return SortValue.CompareTo(orderGroup.SortValue);
            return orderGroup.SortValue.CompareTo(this.SortValue);
        }

	
        private static List<OrderGroup> Sort(List<OrderGroup> orderGroupList)
        {
            //Sort order in each group, newest in time on top
            List<OrderGroup> newOrderGroupList = new List<OrderGroup>();
            foreach(OrderGroup orderGroup in orderGroupList){
                //List<Order> orderList = orderGroup.OrderList;
                orderGroup.Sort();
               // orderGroup.OrderList = orderList;
                newOrderGroupList.Add(orderGroup);
            }
		
            //Sort group
            newOrderGroupList.Sort();
            return newOrderGroupList;
        }

        public static List<OrderGroup> Append(List<Order> orderList, List<OrderGroup> toOrderGroupList)
        {
            List<Order> allOrderList = new List<Order>(orderList);
            foreach(OrderGroup orderGroup in toOrderGroupList)
                allOrderList.AddRange(orderGroup);
            
            return Convert(allOrderList);
        }

        public static List<OrderGroup> Convert(List<Order> orderList){
            Dictionary<string, OrderGroup> orderGroupMap = new Dictionary<string, OrderGroup>();
            string format1 = Constant.FORMAT_DATE_TIME;
            string format2 = Constant.FORMAT_DATE;
	    
            foreach (Order order in orderList) {
                OrderGroup orderGroup = null;
                bool ok = orderGroupMap.TryGetValue(order.GroupedBy, out orderGroup);
                if(orderGroup == null || !ok){
                    orderGroup = new OrderGroup();
                    orderGroup.Title = order.GroupedBy;
                    DateTime date;
                    try
                    {
                        date = DateTime.ParseExact(order.CreatedAt, format1, null);
                        orderGroup.SortValue = date.ToString(format2);
                    } 
                    catch (FormatException e) 
                    {
                        // TODO Auto-generated catch block
                        Debug.WriteLine(e.Message);
                    } catch (ArgumentNullException e) {
                        // TODO Auto-generated catch block
                        Debug.WriteLine(e.Message);
                    }

                    orderGroupMap.Add(order.GroupedBy, orderGroup);
                }
	        
                orderGroup.Add(order);
               
            }
	    
            return Sort(new List<OrderGroup>(orderGroupMap.Values));
        }

       
    }
}
