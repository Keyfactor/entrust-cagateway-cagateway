using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public abstract class EntrustBaseResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public EntrustMessage Message { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
    }

    public class EntrustMessage
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
        public string value { get; set; }
        [JsonProperty("details")]
        public EntrustDetail[] Details { get; set; }
        public string property1 { get; set; }
        public string property2 { get; set; }

    }

    public class EntrustDetail
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
        public string value { get; set; }
        public string property1 { get; set; }
        public string property2 { get; set; }
    }

}
