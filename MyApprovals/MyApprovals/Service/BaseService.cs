using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using MyApprovals.Util;

namespace MyApprovals.Service
{
    abstract class BaseService
    {
        protected string errorNull;
	    protected IServiceResultHandler resultHandler;
	    protected bool useCache = false;
	
	    public BaseService(IServiceResultHandler resultHandler)
	    {
            this.resultHandler = resultHandler;
	    }
	
	    public BaseService(IServiceResultHandler resultHandler, bool useCache)
	    {
		        this.resultHandler = resultHandler;
		    this.useCache = useCache;
	    }
	
	    /*
	    public void executeRequest(String url, HashMap<String, String> params, final int queryId) {		
		    HttpClient client = new HttpClient(this.context);
		    HttpRequestBuilder request = client.get(url).usesCache(true);
		    request.params(params);		
	
		
		    final StringBuilder jsonBuilder = new StringBuilder();
		
		    try {
			    request.to(new HttpResponseHandler(){
				    public void onResponse(HttpResponse response) throws Exception
				    {
					    response.read(jsonBuilder);
					    Result result = parseQueryResult(jsonBuilder.toString(), queryId);
					    if(dataHandler != null)
						    dataHandler.onReceiveResult(result);
				    }			
			    }).execute();
		    } catch (HttpClientException e) {
			    Log.d("BaseSevice.onResponse", "Error " + e.getMessage());
			    if(dataHandler != null)
			    {	
				    Result result = new Result().addItem(new Result.Item(Result.Item.ERROR,e.getLocalizedMessage()));
				    dataHandler.onReceiveResult(result);
			    }
		    }
	    }
	    */
	
	    public void ExecuteRequest(String urlString, List<KeyValuePair<string, string>> args, int serviceId) {	
		    //executeRequestHttpURLConnection(urlString, params, queryId);
            ExecuteRequestAsyncHttpClient(urlString, args, serviceId);
	    }
	
	    protected async void ExecuteRequestAsyncHttpClient(String urlString, List<KeyValuePair<string, string>> args, int serviceId){
            System.Diagnostics.Debug.WriteLine("Request to URL: " + urlString);
            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response = await httpClient.PostAsync(urlString, new FormUrlEncodedContent(args));
            response.EnsureSuccessStatusCode();
           
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Received data: " + responseString);
            ServiceResult serviceResult = GetServiceResult(true, responseString, serviceId);
            //if(this.resultHandler != null)
             //   this.resultHandler.OnReceiveResult(serviceResult);

            //HandleServiceResult(serviceResult);
		   
	    }

        protected ServiceResult GetServiceResult(bool hasResponse, string responseString, int serviceId)
        {
            ServiceResult serviceResult = null;
            if (!hasResponse)
                serviceResult = ParseServiceResult(responseString, serviceId);
            else
                serviceResult = new ServiceResult().AddItem(new ServiceResult.Item(Constant.FAILED, responseString));

            if (serviceResult != null)
                serviceResult.ServiceId = serviceId;

            return serviceResult;
        }

        protected abstract ServiceResult ParseServiceResult(string jsonString, int serviceId);
        protected abstract void HandleServiceResult(ServiceResult serviceResult);

       
    }
}
