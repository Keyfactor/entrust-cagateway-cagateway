using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class CertificateEventListResponse : EntrustBaseResponse
    {
        [JsonProperty("metadata")]
        public Api.Metadata Metadata { get; set; }

        public bool morePages { get; set; }

        public string nextPageIndex { get; set; }

        public List<Api.CertificateEvent> events { get; set; }

        public CertificateEventListResponse()
        {
            events = new List<Api.CertificateEvent>();
        }
    }
}

