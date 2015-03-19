using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApprovals.Util;

namespace MyApprovals.Service
{
    class ServiceResult
    {
        private List<Item> itemList = new List<Item>();
	    private Object data;
	    private int serviceId;

	    public ServiceResult(){}
	
	    public ServiceResult(Item item)
        {
		    this.AddItem(item);
	    }
	
	    public ServiceResult AddItem(Item item)
        {
            this.itemList.Add(item);
		    return this;
	    }

        public int ServiceId { get; set; }

	    public Item GetItem(int index)
        {
            if (index < itemList.Count)
                return itemList.ElementAt(index);
		    return null;
	    }
	
	    public Item GetLastItem()
        {
		    return GetItem(itemList.Count - 1);
	    }
	
	    private const String separator = ", ";
	
	    public override String ToString()
        {
		    StringBuilder builder = new StringBuilder();
            for (int i = 0; i < itemList.Count; i++)
            {
			    builder.Append(itemList.ElementAt(i));
                builder.Append(separator);
		    }
		
		    if(builder.Length > 2)
			    return builder.ToString().Substring(0, builder.Length - 2);
		    
		    return builder.ToString();
	    }
	
	    public bool ContainsError()
        {
            foreach(Item item in this.itemList)
            {
                  if(item.Type == Constant.ERROR)
				    return true;
            }

		    return false;
	    }
	
	    public bool ConstainsFailure()
        {
            foreach (Item item in this.itemList)
            {
                if (item.Type == Constant.FAILED)
				    return true;
		    }
		
		    return false;
	    }
	
	    public bool ContainsErrorOrFailure()
        {
		    foreach (Item item in this.itemList) 
            {
                if (item.Type != Constant.SUCCESS)
				    return true;
		    }
		
		    return false;
	    }

        public Object GetData()
        {
            return data;
        }

	    public ServiceResult SetData(Object data)
        {
		    this.data = data;
		    return this;
	    }

	    public class Item 
        {
		    public Item(int type)
            {
			    Type = type;
		    }
		
		    public Item(int type, string message)
            {
                SetMessage(message);
			    Type = type;
		    }

            public int Type {get;set;}

            private string message;
            public string GetMessage() {return message;}

		    public Item SetMessage(string message)
            {
			    this.message = message;
			    return this;
		    }
		
		   
		    public override string ToString()
            {
			    String strType = "";
                if (Type == Constant.ERROR)
				    strType = "Error: ";
                else if (Type == Constant.FAILED)
				    strType = "Failed: ";
			
			    return strType + this.message;
		    }
		
	    }
    }
}
