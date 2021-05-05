using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust
{
    public class EntrustCAConfig
    { 
        public EntrustCAConfig()
        {
            this.TrackingMap = new Dictionary<string, string>();
        }

        [JsonProperty("ApiEndpoint")]
        public string EntrustEndpoint { get; set; }
        public string CAId { get; set; }
        public ClientCertificate ClientCertificate { get; set; }
        public X509Certificate2 AuthenticationCertificate 
        { 
            get {

                X509Certificate2Collection foundCerts;
                using (var certStore = new X509Store(ClientCertificate.StoreName, ClientCertificate.StoreLocation))
                {
                    certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    foundCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, ClientCertificate.Thumbprint, false);
                    certStore.Close();
                }

                if (foundCerts.Count != 1)
                {
                    throw new FileNotFoundException($"Could not location client auth certificate by thumprint!");
                }

                return foundCerts[0];
            } 
        }
        public Dictionary<string,string> TrackingMap { get; set; }

    }
    public class ClientCertificate
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StoreLocation StoreLocation { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StoreName StoreName { get; set; }
        public string Thumbprint { get; set; }
    }
}
