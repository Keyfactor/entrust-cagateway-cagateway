using CSS.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class EntrustApiClient:LoggingClientBase
    {
        HttpClient RestClient { get; }
        public EntrustApiClient()
        {

        }

        public EntrustApiClient(X509Certificate2 authCert, string endPoint)
        {
            WebRequestHandler handler = new WebRequestHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            handler.ClientCertificates.Add(authCert);

            RestClient = new HttpClient(handler, true)
            {
                BaseAddress = new Uri(endPoint)
            };
        }

        public EntrustApiClient(HttpClient restClient)
        {
            RestClient = restClient;
        }

        public async Task<TResponse> Request<TRequest,TResponse>(IApiRequest apiRequest) where TResponse : EntrustBaseResponse, new()
        {
            HttpResponseMessage response=null;
            switch (apiRequest.Method)
            {
                case HttpMethod m when m == HttpMethod.Get:
                    response = await RestClient.GetAsync(apiRequest.RequestUrl);
                    break;
                case HttpMethod m when m == HttpMethod.Post:
                    response = await RestClient.PostAsJsonAsync(apiRequest.RequestUrl, JsonConvert.DeserializeObject<TRequest>(apiRequest.RequestBody.ToString()));
                    break;
                default:
                    throw new ArgumentException("Invalid HttpMethod in apiRequest","apiRequest");
            }

            var rawResponse = await response.Content.ReadAsStringAsync();           
            var parsedResponse = JsonConvert.DeserializeObject<TResponse>(rawResponse);
            parsedResponse.IsSuccess = response.IsSuccessStatusCode;
            if (!response.IsSuccessStatusCode)
            { 
                parsedResponse.ErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(rawResponse);
            }

            return parsedResponse;
        }
    }
    public class ErrorResponse
    {
        string caMessage { get; set; }
        public EntrustMessage error { get; set; }
    }

    public class EntrustApiException : System.Exception
    {
        public string ApiCode { get; set; }
        public string ApiMessage { get; set; }
        public EntrustApiException(string message) : base(message)
        {
            ApiCode = "unknown";
            ApiMessage = "uknown error occured. See Internal Exception";
        }
        public EntrustApiException(EntrustBaseResponse baseResponse) : base(baseResponse.ErrorResponse.error.message)
        {
            ApiCode = baseResponse.ErrorResponse.error.code;
            ApiMessage = baseResponse.ErrorResponse.error.message;
        }
    }
}
