using Newtonsoft.Json;
using System;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class CertificateEvent
    {
        public string action { get; set; }
        [JsonProperty("event")]
        public int Event { get; set; }
        public DateTime eventDate { get; set; }
        public string certificate { get; set; }
        public string serialNumber { get; set; }
        public string detail { get; set; }
        public string reason { get; set; }
    }
}