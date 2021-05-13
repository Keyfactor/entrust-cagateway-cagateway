using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    
    internal class CADetailRequest : EntrustBaseRequest, IApiRequest
    {
        public CADetailRequest():
            base("GET")
        {

        }
        public CADetailRequest(string caId):
            this()
        {
            CAId = caId;
        }
        
        public string Parameters => throw new NotImplementedException();
        public string RequestUrl => $"v1/certificate-authorities/{CAId}";
        public string CAId { get; set; }
    }
}
