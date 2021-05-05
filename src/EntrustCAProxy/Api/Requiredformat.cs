using Newtonsoft.Json;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Requiredformat
    {
        public string format { get; set; }
        [JsonProperty("protection", NullValueHandling = NullValueHandling.Ignore)]
        public Protection protection { get; set; }
    }


}


