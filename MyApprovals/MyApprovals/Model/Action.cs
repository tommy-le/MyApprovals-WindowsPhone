using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyApprovals.Util;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MyApprovals.Model
{
     [DataContract]
    public class Action : INotifyPropertyChanged
    {
        private string createdAt;
        private string comment;
        private string content;

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
        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                if (value != this.comment)
                {
                    this.comment = value;
                    NotifyPropertyChanged("Comment");
                }
            }
        }

          [DataMember]
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                if (value != this.content)
                {
                    this.content = value;
                    NotifyPropertyChanged("Content");
                }
            }
            
        }
        public Action() { }

        public Action(string createdAt, string comment)
        {
            this.CreatedAt = createdAt;
            this.Comment = comment;
        }

        public static Action fromJObject(JObject jObject)
        {
            try
            {
                Action action = new Action();
                action.Comment = (string)jObject[Constant.KEY_COMMENT];
                action.CreatedAt = (string)jObject[Constant.KEY_CREATED_AT];
                action.Content = action.CreatedAt + " | " + action.Comment;
                return action;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        public static List<Action> fromJArray(JArray jArray)
        {
            List<Action> dataList = new List<Action>();
            if (jArray == null) return dataList;

            foreach (JObject jObject in jArray)
            {
                Action data = fromJObject(jObject);
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
