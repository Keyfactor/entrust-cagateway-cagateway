using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EntrustApi = Keyfactor.AnyGateway.Entrust.Api;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RevokeRequest : EntrustBaseRequest,IApiRequest
    {
        public RevokeRequest() :
            base("POST")
        { 
        
        }

        public RevokeRequest(EntrustApi.Action action):
            base("POST")
        {
            Action = action;
        }

        public RevokeRequest(string caId, string serialNumber, EntrustApi.Action action):
            this(action)
        {
            CAId = caId;
            SerialNumber = serialNumber;
        }

        [JsonProperty("action")]
        public EntrustApi.Action Action { get; set; }
    
        public string CAId { get; set; }
        public string SerialNumber { get; set; }

        public string RequestUrl => $"v1/certificate-authorities/{CAId}/certificates/{SerialNumber}/actions";

        public string Parameters => throw new NotImplementedException();
    }

}
