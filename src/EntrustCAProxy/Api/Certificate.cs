using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Certificate
    {
        public string status { get; set; }
        public string versionNumber { get; set; }
        public string serialNumber { get; set; }
        public string signatureAlgID { get; set; }
        public string issuerName { get; set; }
        public string validityPeriod { get; set; }
        public string subjectName { get; set; }
        public string rfc2253EncodedSubjectName { get; set; }
        public string rfc2253EscapedSubjectName { get; set; }
        public Subjectpublickeyinfo subjectPublicKeyInfo { get; set; }
        public string issuerUniqueIdentifier { get; set; }
        public string subjectUniqueIdentifier { get; set; }
        public string certificateSignatureAlg { get; set; }
        public Revocationinfo revocationInfo { get; set; }
        public string certificateData { get; set; }
    }
}
