using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.Entrust.Client;
using CAProxy.AnyGateway.Interfaces;
using System.Collections.Concurrent;
using CAProxy.AnyGateway.Models;
using System.IO;

[assembly: InternalsVisibleTo("EntrustCAProxy")]
namespace EntrustCAProxyTests
{
    public static class Mocks
    {
        public static ICAConnectorConfigProvider GetMockCAConnectorConfig()
        {
            return new MockCAConnectorConfig();
        }
        public static string GetFileFromResourceFolder(string fileName)
        {
            return File.ReadAllText($"../../resources/{fileName}");
        }
    }

    public class MockCAConnectorConfig : ICAConnectorConfigProvider
    {
        //need to populate with actual details to be able to connect to the CA
        public Dictionary<string, object> CAConnectionData => new Dictionary<string, object>
        {
            ["APIEndpoint"] = "https://mock",
            ["CAId"] = "CA-1001",
            ["ClientCertificate"] = new Dictionary<string, string>
            {
                ["StoreName"] = "My",
                ["StoreName"] = "LocalMachine",
                ["Thumbprint"] = "1234567891323"
            }
        };
    }
}
