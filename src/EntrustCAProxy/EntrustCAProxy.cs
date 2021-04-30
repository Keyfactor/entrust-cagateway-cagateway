using CAProxy.AnyGateway;
using CAProxy.AnyGateway.Interfaces;
using CAProxy.AnyGateway.Models;
using CAProxy.Common;
using CSS.Common.Logging;
using CSS.PKI;
using Keyfactor.AnyGateway.Entrust.Api;
using Keyfactor.AnyGateway.Entrust.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyfactor.AnyGateway.Entrust
{
    public class EntrustCAProxy : BaseCAConnector
    {
        #region Obsolete Methods
        [Obsolete]
        public override EnrollmentResult Enroll(string csr, string subject, Dictionary<string, string[]> san, EnrollmentProductInfo productInfo, CSS.PKI.PKIConstants.X509.RequestFormat requestFormat, RequestUtilities.EnrollmentType enrollmentType)
        {
            throw new NotImplementedException();
        }
        [Obsolete]
        public override void Synchronize(ICertificateDataReader certificateDataReader, BlockingCollection<CertificateRecord> blockingBuffer, CertificateAuthoritySyncInfo certificateAuthoritySyncInfo, CancellationToken cancelToken, string logicalName)
        {
            throw new NotImplementedException();
        }
        #endregion

        private EntrustApiClient ApiClient { get; set; }
        private EntrustCAConfig Config { get; set; }
        /// <summary>
        /// Initialize the Entrust CA Connector with configuration value saved in the AnyGateway database
        /// </summary>
        /// <param name="configProvider"></param>
        public override void Initialize(ICAConnectorConfigProvider configProvider)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                string importedConfig = JsonConvert.SerializeObject(configProvider.CAConnectionData);
                Logger.Trace($"Current CAConnectionData:");
                Logger.Trace($"{importedConfig}");

                Config = JsonConvert.DeserializeObject<EntrustCAConfig>(JsonConvert.SerializeObject(configProvider.CAConnectionData));
                ApiClient = new EntrustApiClient(Config.AuthenticationCertificate,Config.EntrustEndpoint);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing Entrust CA Gateway: {ex.Message}");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                throw;
            }
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
        }

        /// <summary>
        /// Enroll for a certificate via the Entrust CA Gateway API
        /// </summary>
        /// <param name="certificateDataReader"></param>
        /// <param name="csr"></param>
        /// <param name="subject"></param>
        /// <param name="san"></param>
        /// <param name="productInfo"></param>
        /// <param name="requestFormat"></param>
        /// <param name="enrollmentType"></param>
        /// <returns></returns>
        public override EnrollmentResult Enroll(ICertificateDataReader certificateDataReader, string csr, string subject, Dictionary<string, string[]> san, EnrollmentProductInfo productInfo, PKIConstants.X509.RequestFormat requestFormat, RequestUtilities.EnrollmentType enrollmentType)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            try
            {
                var profile = Task.Run(async () => await ApiClient.Request<ProfileResponse>(new ProfileRequest(Config.CAId, productInfo.ProductID))).Result;
                EnrollRequest enrollRequest = new EnrollRequest(Config.CAId,csr,subject,profile);
                var response = Task.Run(async () => await ApiClient.Request<EnrollResponse>(enrollRequest)).Result;
                var cert = new X509Certificate2(Convert.FromBase64String(response.enrollment.body));

                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return new EnrollmentResult() { 
                    CARequestID=cert.SerialNumber,
                    Certificate = response.enrollment.body,
                    StatusMessage = $"Successfully enrolled for certificate {cert.Subject}",
                    Status = 20
                };
            }
            catch (EntrustApiException apiEx)
            {
                Logger.Trace($"API Exception: {apiEx.ApiCode}|{apiEx.ApiMessage}|{apiEx.Message}");
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return new EnrollmentResult
                {
                    Status = 30,
                    StatusMessage = apiEx.Message
                };

            }
            catch (Exception ex)
            {
                Logger.Trace($"General Exception: {ex.Message}");
                Logger.Trace(ex);
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return new EnrollmentResult
                {
                    Status = 30,
                    StatusMessage = ex.Message
                };
            } 
        }

        /// <summary>
        /// Revoke a certificate via the Entrust CA Gateway API
        /// </summary>
        /// <param name="caRequestID">The certificate request id (Serial Number) for the certificate</param>
        /// <param name="hexSerialNumber">The serial number of the certificate being revoked</param>
        /// <param name="revocationReason">The revocation reason selected in Keyfactor</param>
        /// <returns></returns>
        public override int Revoke(string caRequestID, string hexSerialNumber, uint revocationReason)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            Api.Action revokeAction = new Api.Action()
            {
                type = Api.ActionType.RevokeAction,
                reason = GetRevocationReason(revocationReason),
                comment = "Revoked from Keyfactor",
                issueCrl = "true"
            };

            RevokeRequest revokeRequest = new RevokeRequest(Config.CAId, hexSerialNumber, revokeAction);
            var revokeResponse = Task.Run(async () => await ApiClient.Request<RevokeResponse>(revokeRequest)).Result;

            if (!revokeResponse.IsSuccess || 
                revokeResponse.Action.status == Api.ActionStatus.REJECTED)
            {
                Logger.Error($"Unable to revoke certificate with SN:{hexSerialNumber}");
                LogException(revokeResponse.ErrorResponse);
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                throw new EntrustApiException(revokeResponse.ErrorResponse);
            }

            if (revokeResponse.Action.status == Api.ActionStatus.COMPLETED ||
                revokeResponse.Action.status == Api.ActionStatus.ACCEPTED)
            {
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return (int)PKIConstants.Microsoft.RequestDisposition.REVOKED;
            }
            
            //This is included for complier errors. Code should not reach here
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
            return -1;
        }

        /// <summary>
        /// Get a certificate's details from the Entrust CA Gateway API
        /// </summary>
        /// <param name="caRequestID">The serial number of the certificate being requested</param>
        /// <returns></returns>
        public override CAConnectorCertificate GetSingleRecord(string caRequestID)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);
            CertificateRequest certificateRequest = new CertificateRequest(Config.CAId, caRequestID);
            var certificateResponse = Task.Run(async () => await ApiClient.Request<CertificateResponse>(certificateRequest)).Result;

            if (certificateResponse.IsSuccess)
            {
                X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(certificateResponse.Certificate.certificateData));
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                return new CAConnectorCertificate
                {
                    CARequestID = caRequestID,
                    Certificate = certificateResponse.Certificate.certificateData,
                    Status = certificateResponse.Certificate.status == "revoked" ? 21 : 20,
                    RevocationDate = certificateResponse.Certificate.revocationInfo?.revocationDate,
                    ResolutionDate = cert.NotBefore,
                    SubmissionDate = cert.NotBefore,
                    RevocationReason = ParseRecovationReason(certificateResponse.Certificate.revocationInfo?.reason)

                };
            }
            else
            {
                Logger.Error($"Unable to retrieve details certificate SN:{caRequestID}");
                LogException(certificateResponse.ErrorResponse);
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                throw new EntrustApiException(certificateResponse.ErrorResponse);
            }
        }

        /// <summary>
        /// Synchronize Certificates
        /// </summary>
        /// <param name="certificateDataReader"></param>
        /// <param name="blockingBuffer"></param>
        /// <param name="certificateAuthoritySyncInfo"></param>
        /// <param name="cancelToken"></param>
        public override void Synchronize(ICertificateDataReader certificateDataReader, BlockingCollection<CAConnectorCertificate> blockingBuffer, CertificateAuthoritySyncInfo certificateAuthoritySyncInfo, CancellationToken cancelToken)
        {
            Logger.MethodEntry(ILogExtensions.MethodLogLevel.Debug);

            DateTime? startDate = !certificateAuthoritySyncInfo.DoFullSync ? certificateAuthoritySyncInfo.OverallLastSync : new DateTime(2000, 01, 01); ;
            CertificateEventListRequest syncRequest = new CertificateEventListRequest(Config.CAId, startDate);
            BlockingCollection<CertificateEvent> certsToSync = new BlockingCollection<CertificateEvent>(1000);

            try
            {
                Task.Run(async() => await GetCertificateEventsAsync(syncRequest,certsToSync,cancelToken));

                foreach (var cert in certsToSync.GetConsumingEnumerable())  
                {
                    CAConnectorCertificate certToQueue = GetSingleRecord(cert.serialNumber);
                    if (blockingBuffer.TryAdd(certToQueue, 50, cancelToken))
                    {
                        Logger.Trace($"Added {certToQueue.CARequestID} to queue for synchronization");
                    }
                }
                blockingBuffer.CompleteAdding();
            }
            catch (Exception)
            {
                blockingBuffer.CompleteAdding();
                Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
                throw;
            }
            Logger.MethodExit(ILogExtensions.MethodLogLevel.Debug);
        }

        #region Private Methods
        private async Task GetCertificateEventsAsync(CertificateEventListRequest listRequest, BlockingCollection<CertificateEvent> events, CancellationToken cancellationToken)
        {
            CertificateEventListResponse listResponse = new CertificateEventListResponse();
            try
            {
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        events.CompleteAdding();
                        break;
                    }

                    listRequest.NextPageIndex = listResponse.nextPageIndex;
                    listResponse = await ApiClient.Request<CertificateEventListResponse>(listRequest);
                    ProcessCertificateEventResponse(listResponse.events, events, cancellationToken);

                } while (listResponse.morePages);
                events.CompleteAdding();

            }
            catch (EntrustApiException)
            {
                events.CompleteAdding();
                throw;
            }
            catch (Exception)
            {
                events.CompleteAdding();
                throw;
            }
        }
        private void ProcessCertificateEventResponse(List<CertificateEvent> responseEvents, BlockingCollection<CertificateEvent> events, CancellationToken cancellationToken)
        {
            int itemsProcessed = 0;
            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    events.CompleteAdding();
                    break;
                }
                if (events.TryAdd(responseEvents[itemsProcessed], 50, cancellationToken))
                {
                    itemsProcessed++;
                }
            } while (itemsProcessed < responseEvents.Count);
        }
        private void LogException(ErrorResponse response)
        {
            Logger.Error($"Error Code:{response.Message.code} | Description: {response.Message.message}");
        }
        private string GetRevocationReason(uint revocationReason)
        {
            switch (revocationReason)
            {
                case 0:
                    return "unspecified";
                case 1:
                    return "keyCompromise";
                case 3:
                    return "affiliationChanged";
                case 4:
                    return "superseded";
                case 5:
                    return "cessationOfOperation";
                case 6:
                    return "certificateHold";
                default:
                    return "unspecified";
            }
        }
        private int ParseRecovationReason(string revocationReason)
        {
            switch (revocationReason)
            {
                case "unspecified":
                    return 0;
                case "keyCompromise":
                    return 1;
                case "affiliationChanged":
                    return 3;
                case "superseded":
                    return 4;
                case "cessationOfOperation":
                    return 5;
                case "certificateHold":
                    return 6;
                default:
                    return 0;
            }
        }
        #endregion

        /// <summary>
        /// Respond to Ping requests from certutil
        /// </summary>
        public override void Ping()
        {

        }

        /// <summary>
        /// Verify connection to the Entrust CA Gateway when saving configuration via the Set-KeyfactorGatewayConfig cmdlet
        /// </summary>
        /// <param name="connectionInfo"></param>
        public override void ValidateCAConnectionInfo(Dictionary<string, object> connectionInfo)
        {
            
        }

        /// <summary>
        /// Verify template configuration for the Entrust CA Gateway when saving configuation via the Set-KeyfactorGatewayConfig cmdlet
        /// </summary>
        /// <param name="productInfo">Template details from the JSON config file</param>
        /// <param name="connectionInfo">CAConnection section of the JSON config file</param>
        public override void ValidateProductInfo(EnrollmentProductInfo productInfo, Dictionary<string, object> connectionInfo)
        {
            
        }
    }
}
