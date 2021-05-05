using Newtonsoft.Json;
using System;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Tracking
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public Tracking()
        {

        }

        [JsonProperty("tracking.trackingInfo")]
        public string TrackingInfo { get; set; }

        [JsonProperty("tracking.requesterName")]
        public string RequesterName { get; set; }

        [JsonProperty("tracking.requesterEmail")]
        public string RequesterEmail { get; set; }

        [JsonProperty("tracking.requesterPhone")]
        public string RequesterPhone { get; set; }

        [JsonProperty("tracking.additionalEmails")]
        public string AdditionalEmails { get; set; }
    }


}


