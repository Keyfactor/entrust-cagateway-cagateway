using Keyfactor.AnyGateway.Entrust.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EnrollRequest : EntrustBaseRequest, IApiRequest
    {
        public EnrollRequest():
            base("POST")
        {

        }

        public EnrollRequest(string caId, string csr, string subject, ProfileResponse certificateProfile) :
            base("POST")
        {

            this.csr = csr;
            this.requiredFormat = new Api.Requiredformat { format = "PEM" };
            profileId = certificateProfile.Profile.id;
            subjectVariables = ParseSubjectVaraiables(subject, certificateProfile);

            //includeCa = true;
            //properties= new Properties() { property1="",property2="" }; //pull from the template config
            //subjectAltNames = new Subjectaltname[] { new Subjectaltname() {type= "dNSName",value="testenroll1.keyfactor.lab" }, new Subjectaltname() { type = "dNSName", value = "oldfashionedenroll.cocktail.lab" } }
            //previousSubjectDn used to preserv history and key (reissue)
            //optionalCertificateRequestDetails = new Optionalcertificaterequestdetails {
            //    validityPeriod="",//lifetime of the cert P1Y3M10DT0H0M
            //    subjectDn ="" //complete subject. not clear if this will override the subject variables above?
        }
        
        public string CAId { get; set; }

        [JsonProperty("profileId")]
        public string profileId { get; set; }
        [JsonProperty("requiredFormat")]
        public Requiredformat requiredFormat { get; set; }
        [JsonProperty("subjectVariables")]
        public Subjectvariable[] subjectVariables { get; set; }
        [JsonProperty("subjectAltNames")]
        public Subjectaltname[] subjectAltNames { get; set; }
        [JsonProperty("previousSubjectDn", NullValueHandling = NullValueHandling.Ignore)]
        public string previousSubjectDn { get; set; }
        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public Properties properties { get; set; }
        [JsonProperty("csr")]
        public string csr { get; set; }
        [JsonProperty("includeCa",DefaultValueHandling=DefaultValueHandling.Populate)]
        public bool includeCa { get; set; }
        [JsonProperty("optionalCertificateRequestDetails", NullValueHandling = NullValueHandling.Ignore)]
        public Optionalcertificaterequestdetails optionalCertificateRequestDetails { get; set; }

        public string Parameters => throw new NotImplementedException();

        public string RequestUrl => $"v1/certificate-authorities/{CAId}/enrollments";

        private Subjectvariable[] ParseSubjectVaraiables(string subject, ProfileResponse profile)
        {
            Org.BouncyCastle.Asn1.X509.X509Name parsedSubject = new Org.BouncyCastle.Asn1.X509.X509Name(subject);
            List<Subjectvariable> response = new List<Subjectvariable>();
            Org.BouncyCastle.Asn1.DerObjectIdentifier rdn;

            foreach (var subjectVariable in profile.Profile.subjectVariableRequirements)
            {
                if (Org.BouncyCastle.Asn1.X509.X509Name.DefaultLookup.ContainsKey(subjectVariable.name.ToLower()))
                {
                    rdn = (Org.BouncyCastle.Asn1.DerObjectIdentifier)Org.BouncyCastle.Asn1.X509.X509Name.DefaultLookup[subjectVariable.name.ToLower()];
                }
                else
                {
                    throw new Exception($"Unknown RDN value {subjectVariable.name}");
                }

                ICollection subjectValues = parsedSubject.GetValueList(rdn);

                if (subjectVariable.required && subjectValues.Count == 0)
                {
                    throw new Exception($"{subjectVariable.name} is required and missing from the enrollment request");
                }

                foreach (var v in subjectValues)
                {
                    response.Add(new Subjectvariable
                    {
                        type = subjectVariable.name,
                        value = (string)v
                    });
                }
            }
            return response.ToArray();
        }
    }
}




