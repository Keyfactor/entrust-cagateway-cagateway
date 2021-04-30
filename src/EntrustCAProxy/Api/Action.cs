using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Action
    {
        public ActionType type { get; set; }
        public string reason { get; set; }
        public string comment { get; set; }
        public ActionStatus status { get; set; }
        public string issueCrl { get; set; }
        public Properties properties { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionType
    { 
        RevokeAction,
        HoldAction,
        UnholdAction,
        ReactivateAction
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionStatus
    {
        ACCEPTED,
        REJECTED,
        COMPLETED
    }
}
