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

        public async Task<T> Request<T>(IApiRequest apiRequest) where T : EntrustBaseResponse
        {
            HttpResponseMessage response=null;
            switch (apiRequest.Method)
            {
                case HttpMethod m when m== HttpMethod.Get:
                    response = await RestClient.GetAsync(apiRequest.RequestUrl);
                    break;
                case HttpMethod m when m == HttpMethod.Post:
                    response = await RestClient.PostAsJsonAsync(apiRequest.RequestUrl, apiRequest.RequestBody);
                    break;
                default:
                    throw new ArgumentException("Invalid HttpMethod in apiRequest","apiRequest");
            }

            var parsedResponse = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            parsedResponse.IsSuccess = response.IsSuccessStatusCode;
            return parsedResponse;
        }
    }
    public class ErrorResponse : EntrustBaseResponse
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
        public EntrustApiException(ErrorResponse errorResponse) : base(errorResponse.Message.message)
        {
            ApiCode = errorResponse.error.code;
            ApiMessage = errorResponse.error.message;
        }
    }
}
