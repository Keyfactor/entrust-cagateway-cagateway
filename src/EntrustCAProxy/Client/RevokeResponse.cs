using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class RevokeResponse:EntrustBaseResponse
    {
        public RevokeResponse()
        {

        }
        [JsonProperty("action")]
        public Entrust.Api.Action Action { get; set; }
    }
}
