namespace Keyfactor.AnyGateway.Entrust.Api
{
    public class Optionalcertificaterequestdetails
    {
        public string subjectDn { get; set; }
        public Extension[] extensions { get; set; }
        public string validityPeriod { get; set; }
        public int privateKeyUsagePercentage { get; set; }
        public string encodedPublicKey { get; set; }
        public string encodedPrivateKey { get; set; }
        public bool forceIssuance { get; set; }
        public bool useSANFromCSR { get; set; }
        public bool performPOP { get; set; }
    }


}


