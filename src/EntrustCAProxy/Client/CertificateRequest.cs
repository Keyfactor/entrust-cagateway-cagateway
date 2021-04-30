using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CertificateRequest : EntrustBaseRequest, IApiRequest
    {
        public CertificateRequest():
            base("GET")
        {

        }
        public CertificateRequest(string caId, string serialNumber) :
            this()
        {
            CAId = caId;
            SerialNumber = serialNumber;
        }

        public string CAId { get; set; }
        public string SerialNumber { get; set; }
        public string RequestUrl => $"v1/certificate-authorities/{CAId}/certificates/{SerialNumber}";
        public string Parameters { get; set; }
    }
}
