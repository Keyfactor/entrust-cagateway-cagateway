using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    public class CertificateEventListRequest : EntrustBaseRequest, IApiRequest
    {
        public DateTime? StartDate { get; set; } = new DateTime(2000, 01, 01);
        private string StartDateFormatQueryString => String.Format("?startDate={0:yyyy-MM-ddTHH:mm:ss.sssZ}", StartDate);

        public string NextPageIndex { get; set; }
        private string NextPageIndexFormatQueryString => String.Format("?nextPageIndex={0}", NextPageIndex);

        public string Parameters  => String.IsNullOrEmpty(NextPageIndex) ? StartDateFormatQueryString : NextPageIndexFormatQueryString;

        public CertificateEventListRequest():
            base("GET")
        {

        }
        public CertificateEventListRequest(string caId, DateTime? startDate) :
            this(caId, startDate, string.Empty)
        {
        
        }

        public CertificateEventListRequest(string caId, DateTime? startDate, string nextPageIndex) :
            this()
        {
            CAId = caId;
            StartDate = startDate;
            NextPageIndex = nextPageIndex;
        }

        public string CAId { get; set; }

        public string RequestUrl => $"v1/certificate-authorities/{CAId}/certificate-events{this.Parameters}";
    }
}
