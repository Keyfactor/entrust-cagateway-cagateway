using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using System.Runtime.CompilerServices;
using Keyfactor.AnyGateway.Entrust;
using CAProxy.AnyGateway.Interfaces;
using System.Collections.Concurrent;
using CAProxy.AnyGateway.Models;
using System.Threading;
using System.Net.Http;
using Moq.Protected;
using System.Threading.Tasks;
using Keyfactor.AnyGateway.Entrust.Client;
using api = Keyfactor.AnyGateway.Entrust.Api;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("EntrustCAProxy")]
namespace EntrustCAProxyTests
{
    [TestClass]
    public class EntrustSyncronizeTests
    {
        [TestMethod]
        public void ProxyReturnsCertificatesDuringFullSync()
        {
            var mockCertDb = new Mock<ICertificateDataReader>();

            var mockBuffer = new Mock<BlockingCollection<CAConnectorCertificate>>();

            var mockSyncInfo = new Mock<CertificateAuthoritySyncInfo>();
            //mockSyncInfo.Setup(s => s.DoFullSync).Returns(true);

            EntrustCAProxy syncProxy = new EntrustCAProxy();
            syncProxy.Initialize(Mocks.GetMockCAConnectorConfig());
            syncProxy.Synchronize(mockCertDb.Object, mockBuffer.Object, mockSyncInfo.Object, new CancellationToken());

            Assert.IsTrue(mockBuffer.Object.Count > 0);
        }
    }

    [TestClass]
    public class EntrustApiClientTests
    {
        [TestMethod]
        public async Task ApiClientSendsHttpGet()
        {
            var mockWebRequestHandler = new Mock<WebRequestHandler>();


            mockWebRequestHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(Mocks.GetFileFromResourceFolder("CertificateResponse.json"))//expected JSON Response
                })
                .Verifiable();

            EntrustApiClient client = new EntrustApiClient(new HttpClient(mockWebRequestHandler.Object)
            { BaseAddress = new Uri("http://localhost") }
                );
            CertificateRequest request = new CertificateRequest("CA-Jupiter", "5912bd91");

            var result = await client.Request<CertificateResponse>(request);

            mockWebRequestHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
                );

            Assert.AreEqual("5912bd91", result.Certificate.serialNumber);

        }

        [TestMethod]
        public async Task ApiClientSendsHttpPost()
        {
            var mockWebRequestHandler = new Mock<WebRequestHandler>();


            mockWebRequestHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(Mocks.GetFileFromResourceFolder("RevokeResponse.json"))//expected JSON Response
                })
                .Verifiable();

            EntrustApiClient client = new EntrustApiClient(
                new HttpClient(mockWebRequestHandler.Object)
                {
                    BaseAddress = new Uri("http://localhost")
                });

            RevokeRequest request = new RevokeRequest("CA-Jupiter", "5912bd91",
                    new api.Action() {
                        type = api.ActionType.RevokeAction,
                        comment = "Revocation Test",
                        reason = "unspecified"
                    }
                );

            var result = await client.Request<RevokeResponse>(request);

            mockWebRequestHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
                );


            Assert.IsTrue(result.Action.status == api.ActionStatus.COMPLETED ||
                result.Action.status == api.ActionStatus.ACCEPTED);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A HttpMethod of PUT was allowed.")]
        public async Task ApiClientRejectsHttpPut()
        {
            await ApiClientRejectsInvalidHttpVerb(HttpMethod.Put);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "A HttpMethod of DELETE was allowed.")]
        public async Task ApiClientRejectsHttpDelete()
        {
            await ApiClientRejectsInvalidHttpVerb(HttpMethod.Delete);
        }

        private async Task ApiClientRejectsInvalidHttpVerb(HttpMethod method)
        {
            var mockWebRequestHandler = new Mock<WebRequestHandler>();

            mockWebRequestHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(Mocks.GetFileFromResourceFolder("RevokeResponse.json"))
                })
                .Verifiable();

            var mockRequest = new Mock<IApiRequest>();
            mockRequest.Setup(f => f.Method).Returns(method);

            EntrustApiClient client = new EntrustApiClient(
                new HttpClient(mockWebRequestHandler.Object)
                {
                    BaseAddress = new Uri("http://localhost")
                });


            var result = await client.Request<RevokeResponse>(mockRequest.Object);//Should throw an Arugment exception

            Assert.Fail();

        }
    }
}
