using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class ProfileResponse : EntrustBaseResponse
    {
        [JsonProperty("profile")]
        public Api.Profile Profile { get; set; }
    }
}


    
