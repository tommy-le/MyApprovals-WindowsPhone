using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApprovals.Util;
using MyApprovals.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
//using RestSharp;
using Microsoft.Phone.Notification;
using System.Text;
using Microsoft.Phone.Shell;

namespace MyApprovals.Service
{
    class MagentoService
    {
        //public MagentoService(IServiceResultHandler resultHandler, bool useCache) : base(resultHandler, useCache) 
        //{
        //}

        //public MagentoService(IServiceResultHandler resultHandler) : base(resultHandler)
        //{
        //}

        //protected string errorNull;
	    protected IServiceResultHandler resultHandler;
	    protected bool useCache = false;
	
	    public MagentoService(IServiceResultHandler resultHandler)
	    {
            this.resultHandler = resultHandler;
	    }

        public MagentoService(IServiceResultHandler resultHandler, bool useCache)
	    {
		    this.resultHandler = resultHandler;
		    this.useCache = useCache;
	    }

        public HttpClientHandler Handler { get; private set; }
        private HttpClient GetClient()
        {
            try
            {
                string url = Session.GetSavedSession().StoreURL;
                Uri uri = new Uri(url);

                Handler = new HttpClientHandler();
                Handler.CookieContainer = new CookieContainer();
                Cookie c = CookieStore.GetStoredCookie();
                if (c != null)
                    Handler.CookieContainer.Add(uri, c);

                Handler.UseCookies = true;
                Handler.AllowAutoRedirect = false;

                HttpClient httpClient = new HttpClient(Handler, false);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = uri;

                return httpClient;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

  
        
        public async void ExecuteRequest(string urlString, List<KeyValuePair<string, string>> args, int serviceId)
        {
            Debug.WriteLine("Request to URL: " + urlString);
            bool hasResponse = true;
            string responseString = null;
            HttpClient httpClient = GetClient();
            if (httpClient == null)
            {
                Debug.WriteLine("Cannot initiate HttpClient instance"); ;
                hasResponse = false;
            }
            else
            {
                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(urlString, new FormUrlEncodedContent(args));
                    //HttpResponseMessage response = await httpClient.GetAsync(urlString);
                    response.EnsureSuccessStatusCode();
                    responseString = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Received data: " + responseString);
                    CookieStore.StoreCookies(Handler.CookieContainer.GetCookies(httpClient.BaseAddress));
                }
                catch (HttpRequestException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    hasResponse = false;
                }
                catch (WebException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    hasResponse = false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    hasResponse = false;
                }
            }

            ServiceResult serviceResult = GetServiceResult(hasResponse, responseString, serviceId);
            HandleServiceResult(serviceResult);

            if (this.resultHandler != null)
                this.resultHandler.OnReceiveResult(serviceResult);
            
            
        }

        protected ServiceResult GetServiceResult(bool hasResponse, string responseString, int serviceId)
        {
            ServiceResult serviceResult = null;
            //For test offline only
            if(Constant.TEST_OFFLINE)
                hasResponse = true;
            //End test

            if (hasResponse)
                serviceResult = ParseServiceResult(responseString, serviceId);
            else
                serviceResult = new ServiceResult().AddItem(new ServiceResult.Item(Constant.FAILED, "Network problem or Server did not return response"));

            if (serviceResult != null)
                serviceResult.ServiceId = serviceId;

            return serviceResult;
        }

	    protected ServiceResult ParseServiceResult(string jsonstring, int serviceId) 
        {
		    // TODO Auto-generated method stub
            Debug.WriteLine("Service id = " + serviceId);
		    switch (serviceId) {
		    case Constant.BIZ_LOGIN:
			    if(Constant.TEST_OFFLINE)
			    	jsonstring = "{\"result_code\":1,\"SID\":\"m1u8q869q7tu1t2c3qdr44nms0\",\"result\":{\"quote\":\"505\",\"firstname\":\"Saeed\",\"lastname\":\"Ahmed\",\"address\":{\"street\":\"PO Box 38083\",\"city\":\"Dubai\",\"country\":\"AE\"},\"telephone\":\"043204969\"}}";
				
			    return ParseLogin(jsonstring);

            case Constant.BIZ_GET_APPROVERS:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"can_give_final\":\"false\",\"result_code\":1,\"message\":\"\",\"approvers\":{\"next\":[{\"id\":\"270\",\"name\":\"Emrill AFM\",\"email\":\"emrill.afm@36-strategies.com\"},{\"id\":\"272\",\"name\":\"Emrill SFM\",\"email\":\"emrill.sfm@36-strategies.com\"}],\"optional\":[]}}";

                return ParseGetApprovers(jsonstring);

            case Constant.BIZ_GET_MY_ORDERS:
                if(Constant.TEST_OFFLINE)
                    jsonstring = "{\"result_code\":1,\"SID\":\"m1u8q869q7tu1t2c3qdr44nms0\",\"orders\":[{\"id\":\"1291\",\"increment_id\":\"200000187\",\"contract_id\":\"3\",\"contract_name\":\"Emril test contract\",\"created_at\":\"2014-01-18 17:20:29\",\"updated_at\":\"2014-01-18 17:20:31\",\"grouped_by_date\":\"January 18, 2014\",\"created_time\":\"5:20 PM\",\"total\":\"AED71.40\",\"status\":\"Pending Approval\"},{\"id\":\"1290\",\"increment_id\":\"200000186\",\"contract_id\":\"3\",\"contract_name\":\"Emril test contract\",\"created_at\":\"2014-01-18 17:07:30\",\"updated_at\":\"2014-02-05 17:41:52\",\"grouped_by_date\":\"January 18, 2014\",\"created_time\":\"5:07 PM\",\"total\":\"AED44.00\",\"status\":\"Pending Approval\"},{\"id\":\"1288\",\"increment_id\":\"200000184\",\"contract_id\":\"3\",\"contract_name\":\"Emril test contract\",\"created_at\":\"2014-01-17 07:03:31\",\"updated_at\":\"2014-01-17 16:51:36\",\"grouped_by_date\":\"January 17, 2014\",\"created_time\":\"7:03 AM\",\"total\":\"AED150.00\",\"status\":\"Pending Approval\"}],\"total\":3}";
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";
                return ParseGetMyOrders(jsonstring);

            case Constant.BIZ_GET_ORDER_DETAIL:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"SID\":\"m1u8q869q7tu1t2c3qdr44nms0\",\"order\":{\"id\":\"1291\",\"increment_id\":\"200000187\",\"contract\":\"Emril test contract\",\"ordered_by\":\"Emrill FM\",\"created_date\":\"January 18, 2014 5:20 PM\",\"total\":\"AED71.40\",\"comments\":[{\"created_at\":\"2014-01-19 17:04:57\",\"comment\":\"Order Approved by Marc Daly\"},{\"created_at\":\"2014-01-18 17:26:03\",\"comment\":\"Order Approved by Emrill AFM\"},{\"created_at\":\"2014-01-18 17:23:35\",\"comment\":\"Order Approved by Emrill FM\"},{\"created_at\":\"2014-01-18 17:22:04\",\"comment\":\"Order Approved by Approver  Super\"},{\"created_at\":\"2014-01-18 17:20:31\",\"comment\":\"Emrill FM\"}],\"items\":[{\"name\":\"Halloween Costume1\",\"uom\":\"meter\",\"ordered_qty\":\"1\",\"sub_total\":\"AED22.00\"},{\"name\":\"SAFETY SHOES FOR LABOURERS WITH STEEL TOE CAP - SIZE 40\",\"uom\":\"\",\"ordered_qty\":\"1\",\"sub_total\":\"AED49.40\"}]}}";
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";

                return ParseGetOrderDetail(jsonstring);

            case Constant.BIZ_ADD_COMMENT:
                if(Constant.TEST_OFFLINE)
                    jsonstring = "{\"result_code\":1,\"message\":\"Comment added successfully\"}";
                //	jsonstring = ;
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";

                return ParseAddComment(jsonstring);

            case Constant.BIZ_APPROVE:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"message\":\"Order approved successfully\"}";
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";
                return ParseApprove(jsonstring);

            case Constant.BIZ_APPROVE_NEXT:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"message\":\"Order approved successfully\"}";
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";
                return ParseApprove(jsonstring);

            case Constant.BIZ_APPROVE_OPTIONAL:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"message\":\"Order approved successfully\"}";
                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";
                return ParseApprove(jsonstring);

            case Constant.BIZ_REJECT:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"message\":\"Order rejected successfully\"}";

                //jsonstring = "{\"result_code\":2,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}";

                return ParseReject(jsonstring);

            case Constant.BIZ_REGISTER_DEVICE:
                if(Constant.TEST_OFFLINE)
                	jsonstring = "{\"result_code\":1,\"message\":\"Device registered successfully\"}";

                return ParseCommonResponse(jsonstring, Constant.BIZ_REGISTER_DEVICE);
		
		    default:
			    break;
		    }
		
		    return null;
	    }

	    
	    protected void HandleServiceResult(ServiceResult result)
        {
		    // TODO Auto-generated method stub
            if (result == null) return;

		    if(result.ServiceId == Constant.BIZ_LOGIN)
            {
	            Session session = Session.GetSavedSession();
	            if(result.ContainsErrorOrFailure())
                {
                    session.WrongLogin = true;
	            }
	            else
                {
                    session.Sid = (string)result.GetData();
                    session.LoggedIn = true;
                    session.WrongLogin = false;

                    //byte[] bDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
                    //string deviceID = Convert.ToBase64String(bDeviceID);
                    //Debug.WriteLine("Device ID = {0}", deviceID);
                    //RegisterDevice(deviceID);

	            }
	        
	            session.Save();
		    }
            else if (result.ServiceId == Constant.BIZ_SEND_PUSH_URI)
            {
                if (!result.ContainsErrorOrFailure())
                    PhoneApplicationService.Current.State[Constant.KEY_PUSH_URI] = null;
            }
	    }
	
	    public void RegisterDevice(string pushUri)
        {
            byte[] bDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            string deviceID = Convert.ToBase64String(bDeviceID);

            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "registerdevice";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>> 
                                                        {   new KeyValuePair<string, string>("deviceos", "wp"),
                                                            new KeyValuePair<string, string>("devicetoken", deviceID),
                                                            new KeyValuePair<string, string>("pushurl", HttpUtility.UrlEncode(pushUri)),
                                                            new KeyValuePair<string, string>("sid", session.Sid)};

            ExecuteRequest(url, args, Constant.BIZ_REGISTER_DEVICE);
	    }

        //public void SendPushUri(string pushUri)
        //{
           
        //    Session session = Session.GetSavedSession();
        //    string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "pushuri";
        //    List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>> 
        //                                                {  
        //                                                    new KeyValuePair<string, string>("deviceos", "wp"),
        //                                                    new KeyValuePair<string, string>("devicetoken", deviceID),
        //                                                    new KeyValuePair<string, string>("uri", pushUri)};

        //    ExecuteRequest(url, args, Constant.BIZ_SEND_PUSH_URI);
        //}
	
	    public void Login(string userId, string password, string storeURL)
        {
		    Session session = Session.GetSavedSession();
		    session.StoreURL = StringUtil.GetURL(storeURL);
            session.UserId = userId;
            session.Save();
		
		    session = Session.GetSavedSession();

            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "login";//?u=" + userId + "&p=" + password;
		    List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("u", userId),
                                                            new KeyValuePair<string, string>("p", password)};

		    ExecuteRequest(url, args, Constant.BIZ_LOGIN);
	    }
	
        public void GetMyOrders(int limit, int page)
        {
            Session session = Session.GetSavedSession();
            Debug.WriteLine("sid="+session.Sid);
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "cart/latestorder";///sid=" + session.Sid;
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("limit", limit.ToString()),
                                                            new KeyValuePair<string, string>("p", page.ToString())};

            ExecuteRequest(url, args, Constant.BIZ_GET_MY_ORDERS);
        }
	
        public void GetOrderDetail(string orderId)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "cart/orderdetail";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId)};

            ExecuteRequest(url, args, Constant.BIZ_GET_ORDER_DETAIL);
        }
	
        public void GetApprovers(string orderId)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "listnextapprovers";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId)};

            ExecuteRequest(url, args, Constant.BIZ_GET_APPROVERS);
        }
	
        public void AddComment(string orderId, string comment)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "addcomment";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId),
                                                            new KeyValuePair<string, string>("comment", comment)};

            ExecuteRequest(url, args, Constant.BIZ_ADD_COMMENT);
        }
	
        public void NextApprover(string orderId, string approverId)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "nextapprover";
            if(approverId == null)
                approverId = "";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId),
                                                            new KeyValuePair<string, string>("appid", approverId)};

            ExecuteRequest(url, args, Constant.BIZ_APPROVE_NEXT);
        }
	
        public void OptionalApprover(string orderId, string approverId)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "nextoptapprover";
            if(approverId == null)
                approverId = "";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId),
                                                            new KeyValuePair<string, string>("appid", approverId)};

            ExecuteRequest(url, args, Constant.BIZ_APPROVE_OPTIONAL);
        }
	
        public void Reject(string orderId, string comment)
        {
            Session session = Session.GetSavedSession();
            string url = StringUtil.GetURL(session.StoreURL) + Constant.APP_URL + "rejectorder";
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string,string>> 
                                                        {   new KeyValuePair<string, string>("sid", session.Sid),
                                                            new KeyValuePair<string, string>("orderid", orderId),
                                                            new KeyValuePair<string, string>("comment", comment)};

            ExecuteRequest(url, args, Constant.BIZ_REJECT);
        }
	
	    private ServiceResult ParseValidJSON(string jsonstring, int serviceId)
        {
		    ServiceResult result = new ServiceResult();
            result.ServiceId = serviceId;
	        
            if(jsonstring == null){
	    	    result.AddItem(new ServiceResult.Item(Constant.ERROR, "Invalid returned data from the server. Please try again later!"));
	    	    return result;
	        }

            try
            {
                JObject JObject = JObject.Parse(jsonstring);
                if (JObject == null)
                    result.AddItem(new ServiceResult.Item(Constant.ERROR, "Invalid returned data from the server. Please try again later!"));
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                Debug.WriteLine(e.Message);
                result.AddItem(new ServiceResult.Item(Constant.ERROR, e.Message));
            }

            return result;
	    }
	
	    /*
	     {\"result_code\":\"0\",\"result\":{\"message\":\"please check user and password\"}}
	    {\"result_code\":\"1\",\"SID\":\"mej84hgvsjlr2usstbabq9fak2\",\"result\":{\"quote\":\"1460\",\"firstname\":\"Approver \",\"lastname\":\"Super\",\"address\":\"\"}}
	     */

	    private ServiceResult ParseLogin(string jsonstring)
        {
		    ServiceResult result = ParseValidJSON(jsonstring, Constant.BIZ_LOGIN);
		    if(result.ContainsErrorOrFailure())
			    return result;
		   
		    JObject responseObj = JObject.Parse(jsonstring);
		   
		    int resultCode = (int)responseObj[Constant.KEY_RESULT_CODE];
		    if(resultCode == Constant.VALUE_OK){			   	
			   	string sid = (string)responseObj[Constant.KEY_SID];
			   	return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS)).SetData(sid);
		    }

            string message = (string)responseObj[Constant.KEY_MSG];
            return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message));	   
	    }
	
	    /*
	     {\"result_code\":1,
	     \"SID\":\"camh8o73341tq5jjig04s00r36\",
	     \"orders\":
	 	    [{\"id\":\"1285\",
	 	    \"increment_id\":\"200000181\",\"contract_id\":\"3\",\"contract_name\":\"Emril test contract\",\"created_at\":\"2014-01-16 04:43:18\",\"updated_at\":\"2014-01-16 10:49:22\",\"grouped_by_date\":\"January 16, 2014\",\"created_time\":\"4:43 AM\",\"total\":\"AED22.00\",\"status\":\"Pending Approval\"},{\"id\":\"628\",\"increment_id\":\"100000513\",\"contract_id\":\"1\",\"contract_name\":\"11925 - Downtown\",\"created_at\":\"2013-07-22 05:10:24\",\"updated_at\":\"2014-01-16 10:58:43\",\"grouped_by_date\":\"July 22, 2013\",\"created_time\":\"5:10 AM\",\"total\":\"AED100,001.00\",\"status\":\"Pending Approval\"}]}

	 
	     {\"result_code\":0,\"SID\":\"1hs5g8th1v1efrvmvk93g03o46\",\"message\":\"Please log in\"}
	     */
        private ServiceResult ParseGetMyOrders(string jsonstring)
        {
            ServiceResult result = ParseValidJSON(jsonstring, Constant.BIZ_GET_MY_ORDERS);
            if (result.ContainsErrorOrFailure())
                return result;

            try
            {
                JObject responseObj = JObject.Parse(jsonstring);

                int resultCode = (int)responseObj[Constant.KEY_RESULT_CODE];
                if (resultCode == Constant.VALUE_OK)
                {
                    JArray orderArrary = (JArray)responseObj[Constant.KEY_ORDERS];
                    List<Order> orderList = Order.Parse(orderArrary);
                    //List<OrderGroup> orderGroupList = OrderGroup.Convert(orderList);
                    return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS)).SetData(orderList);
                }

                // if(responseObj.has(Constant.KEY_RESULT))
                string message = (string)responseObj[Constant.KEY_MSG];
                ServiceResult serviceResult = new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message));
                if (resultCode == Constant.VALUE_EXPIRED)
                    serviceResult.SetData(resultCode.ToString());

                return serviceResult;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, e.Message));
            }
        }

        /*
         {\"result_code\":1,\"SID\":\"camh8o73341tq5jjig04s00r36\",
	 
         \"order\":{\"id\":\"1285\",\"increment_id\":\"200000181\",\"contract\":\"Emril test contract\",\"ordered_by\":\"Victor Soren\",\"created_date\":\"January 16, 2014 4:43 AM\",\"total\":\"AED22.00\",\"comments\":[{\"created_at\":\"2014-01-16 10:49:22\",\"comment\":\"Rrrrrgg\"},{\"created_at\":\"2014-01-16 10:45:51\",\"comment\":\"Test\"},{\"created_at\":\"2014-01-16 10:37:23\",\"comment\":\"Test tiep\"},{\"created_at\":\"2014-01-16 04:43:20\",\"comment\":\"Victor Soren\"}],\"items\":[{\"name\":\"Halloween Costume1\",\"uom\":\"meter\",\"ordered_qty\":\"1\",\"sub_total\":\"AED22.00\"}]}}
         */
        private ServiceResult ParseGetOrderDetail(string jsonstring)
        {
            ServiceResult result = ParseValidJSON(jsonstring, Constant.BIZ_GET_ORDER_DETAIL);
            if (result.ContainsErrorOrFailure())
                return result;

            try
            {
                JObject responseObj = JObject.Parse(jsonstring);

                int resultCode = (int)responseObj[Constant.KEY_RESULT_CODE];
                if (resultCode == Constant.VALUE_OK)
                {
                    JObject jObject = (JObject)responseObj[Constant.KEY_ORDER];
                    Order order = Order.Parse(jObject);
                    return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS)).SetData(order);
                }

                //JObject resultObj = responseObj.getJObject(Constant.KEY_RESULT);
                string message = (string)responseObj[Constant.KEY_MSG];
                ServiceResult serviceResult = new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message));
                if (resultCode == Constant.VALUE_EXPIRED)
                    serviceResult.SetData(resultCode.ToString());

                return serviceResult;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, e.Message));
            }
        }


        private ServiceResult ParseGetApprovers(string jsonstring)
        {
            ServiceResult result = ParseValidJSON(jsonstring, Constant.BIZ_GET_APPROVERS);
            if (result.ContainsErrorOrFailure())
                return result;

            try
            {
                JObject responseObj = JObject.Parse(jsonstring);

                int resultCode = (int)responseObj[Constant.KEY_RESULT_CODE];
                if (resultCode == Constant.VALUE_OK)
                {
                    JObject approversObj = (JObject)responseObj[Constant.KEY_APPROVERS];
                    bool canGiveFinal = (bool)responseObj[Constant.KEY_CAN_GIVE_FINAL];

                    JArray nextArrary = (JArray)approversObj[Constant.KEY_NEXT_APPROVER];
                    JArray optionalArrary = (JArray)approversObj[Constant.KEY_OPTIONAL_APPROVER];

                    List<Approver> nextList = Approver.fromJArray(nextArrary);
                    List<Approver> optionalList = Approver.fromJArray(optionalArrary);

                    Order order = new Order();
                    order.NextApproverList = nextList;
                    order.OptionalApproverList = optionalList;
                    order.CanGiveFinal = canGiveFinal;

                    return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS)).SetData(order);
                }

                string message = (string)responseObj[Constant.KEY_MSG];
                ServiceResult serviceResult = new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message));
                if (resultCode == Constant.VALUE_EXPIRED)
                    serviceResult.SetData(resultCode.ToString());

                return serviceResult;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, e.Message));
            }

        }
	
	    private ServiceResult ParseCommonResponse(string jsonstring, int bizType){
		    ServiceResult result = ParseValidJSON(jsonstring, bizType);
		    if(result.ContainsErrorOrFailure())
			    return result;


            JObject responseObj = JObject.Parse(jsonstring);

            string message = (string)responseObj[Constant.KEY_MSG];
			  
		       int resultCode = (int)responseObj[Constant.KEY_RESULT_CODE];
		       if(resultCode == Constant.VALUE_OK){			   	
			       if(message != null)
                       return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS, message));
                   return new ServiceResult().AddItem(new ServiceResult.Item(Constant.SUCCESS));
		       }
		       else if(resultCode == Constant.VALUE_EXPIRED){
                   return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message)).SetData(resultCode);
		       }
		       //JObject resultObj = responseObj.getJObject(Constant.KEY_RESULT);
		   
		       return new ServiceResult().AddItem(new ServiceResult.Item(Constant.ERROR, message));
	 		 
		    	
	    }
	
	    private ServiceResult ParseReject(string jsonstring){
		    return ParseCommonResponse(jsonstring, Constant.BIZ_REJECT);
	    }
	
	    private ServiceResult ParseApprove(string jsonstring){
		    return ParseCommonResponse(jsonstring, Constant.BIZ_APPROVE);
	    }
	
	    private ServiceResult ParseAddComment(string jsonstring){
		    return ParseCommonResponse(jsonstring, Constant.BIZ_ADD_COMMENT);
        }

    }
}
