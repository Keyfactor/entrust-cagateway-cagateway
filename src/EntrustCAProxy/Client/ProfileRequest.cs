using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class ProfileRequest : EntrustBaseRequest,IApiRequest
    {
        public ProfileRequest():
            base("GET")
        {

        }

        public ProfileRequest(string caId, string profileId) :
            this()
        {
            CAId = caId;
            ProfileId = profileId;
        }


        public string CAId { get; set; }
        public string ProfileId { get; set; }

        public string RequestUrl => $"v1/certificate-authorities/{CAId}/profiles/{ProfileId}";

        public string Parameters => throw new NotImplementedException();
    }
}
