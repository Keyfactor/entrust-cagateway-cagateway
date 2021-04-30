using System;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Revocationinfo
    {
        public string comment { get; set; }
        public DateTime compromiseDate { get; set; }
        public DateTime revocationDate { get; set; }
        public string reason { get; set; }
    }
}
