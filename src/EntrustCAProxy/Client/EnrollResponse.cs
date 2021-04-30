using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class EnrollResponse:EntrustBaseResponse
    {
        public Api.Enrollment enrollment{ get; set; }
    }
}
