using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntrustBaseRequest
    {
        public HttpMethod Method { get; set; }

        public object RequestBody => JsonConvert.SerializeObject(this);

        public EntrustBaseRequest(string httpMethod)
        {
            if (String.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            switch (httpMethod.ToUpper())
            {
                case "GET":
                    Method = HttpMethod.Get;
                    break;
                case "POST":
                    Method = HttpMethod.Post;
                    break;
                default:
                    throw new ArgumentException($"{httpMethod} is not a supported HTTP request verb.", "httpMethod");
            }
        }

    }

    public interface IApiRequest 
    {
        HttpMethod Method { get; set; }
        string Parameters { get; }
        string RequestUrl { get; }
        object RequestBody { get; }
    }


    public class EntrustField
    {
        public string Id { get; set; }
        public FieldType FieldType { get; set; }
        public string FieldValue { get; set; }
    }

    public enum FieldType
    { 
        Query,
        Path
    }
}
