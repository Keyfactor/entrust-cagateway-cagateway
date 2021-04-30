using Newtonsoft.Json;
using System;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Tracking
    {
        public Tracking()
        {

        }
        [JsonProperty("trackingInfo")]
        public string TrackingInfo { get; set; }

        [JsonProperty("requesterName")]
        public string RequesterName { get; set; }

        [JsonProperty("requesterEmail")]
        public string RequesterEmail { get; set; }

        [JsonProperty("requesterPhone")]
        public string RequesterPhone { get; set; }

        [JsonProperty("deactivated")]
        public bool Deactivated { get; set; }

        /// <summary>
        /// The date and time the certificate was last deactivated.  This is a read-only field and is returned only if deactivated&#x3D;true. 
        /// </summary>
        /// <value>The date and time the certificate was last deactivated.  This is a read-only field and is returned only if deactivated&#x3D;true. </value>
        [JsonProperty("deactivatedOn")]
        public DateTime? DeactivatedOn { get; set; }
    }


}


