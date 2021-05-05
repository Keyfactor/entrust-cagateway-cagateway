using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    internal class CADetailResponse:EntrustBaseResponse
    {
        public CADetail ca { get; set; }
    }

    internal class CADetail
    {
        [JsonProperty("id")]
        public string CAId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("properties")]
        public CAProperties properties { get; set; }
        public CAType GetCAType()
        {
            switch (this.properties.connector_name)
            {
                case "com.entrust.SecurityManager":
                case "com.entrust.MicrosoftCA":
                    return CAType.Private;
                case "com.entrust.ECS":
                    return CAType.Public;
                default:
                    return CAType.Private;
            }
        }
    }

    internal class CAProperties
    {
        public string issuer_dn { get; set; }
        public string connector_name { get; set; }
        public string type { get; set; }
    }

    internal enum CAType
    {
        Private,
        Public
    }
}
