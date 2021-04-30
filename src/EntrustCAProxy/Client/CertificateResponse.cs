using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class CertificateResponse : EntrustBaseResponse
    {
        public CertificateResponse()
        {

        }

        [JsonProperty("certificate")]
        public Api.Certificate Certificate { get; set; }
    }
}
