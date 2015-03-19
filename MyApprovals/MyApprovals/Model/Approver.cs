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
    public class Approver : INotifyPropertyChanged
    {
         [DataMember]
        public string Name {get;set;}
         [DataMember]
	    public string Id{ get; set;}

        public Approver() { }

        public Approver(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Approver fromJObject(JObject jObject)
        {
            try
            {
                Approver approver = new Approver();
                approver.Id = (string)jObject[Constant.KEY_ID];
                approver.Name = (string)jObject[Constant.KEY_NAME];
                return approver;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }
	
        public static List<Approver> fromJArray(JArray jArray)
        {
		    List<Approver> dataList = new List<Approver>();
            if (jArray == null) return dataList;

            foreach (JObject jObject in jArray)
            {
                Approver data = fromJObject(jObject);
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
