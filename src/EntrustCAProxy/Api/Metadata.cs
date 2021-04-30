using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Metadata
    {
        public int currentCount { get; set; }
        public int totalCount { get; set; }
        public int currentOffset { get; set; }
        public int currentLimit { get; set; }
    }
}
