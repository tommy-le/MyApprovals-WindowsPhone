using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApprovals.Util
{
    class StringUtil
    {
        public static bool IsNullOrEmpty(string str){
            return str == null || "".Equals(str);
	    }
	
	    public static bool IsNullOrEmptyAfterTrimmed(string str){
		    return IsNullOrEmpty(StringUtil.Trim(str));
	    }
	
        //public static bool isNullOrEmpty(List arrayList){
        //    return arrayList == null || arrayList.size() == 0;
        //}
	
	    public static string Trim(string str){
		    if(str == null) return str;
            return str.Trim();
	    }
	
	    public static bool Equal(string str1, string str2){
		    if(str1 == null) return str2 == null;
		    return str1.Equals(str2);
	    }
	
	    public static string GetURL(string url){
		    if(url == null) return url;
		    if(!url.StartsWith("http://"))
			    url = "http://" + url;
		    if(url.EndsWith("/"))
			    url = url.Substring(0, url.Length - 1);
		    return url;
	    }
    }
}
