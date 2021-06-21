# Entrust CA Gateway
## Ca-gateway

This AnyGateway implementation consumes the Entrust CA Gateway API to provide access to the Entrust Certificate Services, Entrust Authority Security Manager, and Entrust Managed PKI product lines. 

<!-- add integration specific information below -->
*** 
# Introduction
The [Entrust CA Gateway](https://blog.entrust.com/2019/08/its-time-to-rest-easy/) is a RESTful API platform that allows for the issuance, synchronization, and management of CA certificates backed by Entrust's Manged SSL service, public CA offering, and Microsoft CAs.

# Prerequisites

## Certificate Chain

In order to enroll for certificates the Keyfactor Command server must trust the trust chain. Once you create your Root and/or Subordinate CA, make sure to import the certificate chain into the Command Server certificate store

# Install
* Download latest successful build from [GitHub Releases](/releases/latest)

* Copy EntrustCAProxy.dll to the Program Files\Keyfactor\Keyfactor AnyGateway directory

* Update the CAProxyServer.config file
  * Update the CAConnection section to point at the EntrustCAProxy class
  ```xml
  <alias alias="CAConnector" type="Keyfactor.AnyGateway.Entrust.EntrustCAProxy, EntrustCAProxy"/>
  ```

# Configuration
The following sections will breakdown the required configurations for the AnyGatewayConfig.json file that will be imported to configure the AnyGateway.

## Templates
The Template section will map the CA's SSL profile to an AD template. For private CAs there are no addtional parameters required. The ProductID can be found via an authenticated GET /v1/certificate-authorities/{caId}/profiles request to the ApiEndpoint. ```curl -X GET "https://cagw.yourcorp.com/cagw/v1/certificate-authorities/CA-Jupiter/profiles" -H  "accept: application/json"```
 ```json
  "Templates": {
	"WebServer": {
      "ProductID": "f733787d-5649-4c74-b596-3a7f79b6172b",
      "Parameters": {
      }
   }
}
 ```
## Security
The security section does not change specifically for the Entrust CA Gateway.  Refer to the AnyGateway Documentation for more detail.
```json
  /*Grant permissions on the CA to users or groups in the local domain.
	READ: Enumerate and read contents of certificates.
	ENROLL: Request certificates from the CA.
	OFFICER: Perform certificate functions such as issuance and revocation. This is equivalent to "Issue and Manage" permission on the Microsoft CA.
	ADMINISTRATOR: Configure/reconfigure the gateway.
	Valid permission settings are "Allow", "None", and "Deny".*/
    "Security": {
        "Keyfactor\\Administrator": {
            "READ": "Allow",
            "ENROLL": "Allow",
            "OFFICER": "Allow",
            "ADMINISTRATOR": "Allow"
        },
        "Keyfactor\\gateway_test": {
            "READ": "Allow",
            "ENROLL": "Allow",
            "OFFICER": "Allow",
            "ADMINISTRATOR": "Allow"
        },		
        "Keyfactor\\SVC_TimerService": {
            "READ": "Allow",
            "ENROLL": "Allow",
            "OFFICER": "Allow",
            "ADMINISTRATOR": "None"
        },
        "Keyfactor\\SVC_AppPool": {
            "READ": "Allow",
            "ENROLL": "Allow",
            "OFFICER": "Allow",
            "ADMINISTRATOR": "Allow"
        }
    }
```
## CerificateManagers
The Certificate Managers section is optional.
	If configured, all users or groups granted OFFICER permissions under the Security section
	must be configured for at least one Template and one Requester. 
	Uses "<All>" to specify all templates. Uses "Everyone" to specify all requesters.
	Valid permission values are "Allow" and "Deny".
```json
  "CertificateManagers":{
		"DOMAIN\\Username":{
			"Templates":{
				"MyTemplateShortName":{
					"Requesters":{
						"Everyone":"Allow",
						"DOMAIN\\Groupname":"Deny"
					}
				},
				"<All>":{
					"Requesters":{
						"Everyone":"Allow"
					}
				}
			}
		}
	}
```
## CAConnection
The CA Connection section will determine the API endpoint and configuration data used to connect to Entrust CA Gateway. 
* ```ApiEndpoint```
This is the base address of the endpoint used by the Gateway to connect to the API.
* ```CAId```
This is the name of the CA configured within the Entrust CA Gateway that is being integrated with.  This value can be found with an authenticated GET /v1/certificate-authorities request to the ApiEndpoint. ```curl -X GET "https://cagw.yourcorp.com/cagw/v1/certificate-authorities" -H  "accept: application/json"```
* ```ClientCertificate```
This section will determine which certificate is used by the AnyGateway to authenticate to the API.  It will search the Windows Certficaite Store by Location and Thumbprint to find the correct certificate.  The network service account must have access to the Certificate and Key Material for certificate authentication to work. 
* ```TrackingMap```
The TrackingMap configuration parameters are required for Entrust Certificate Services (Public CA) and allows for configurable Enrollment Fields to be defined in Keyfactor Command. These fields must be defined on all templates to be issued from the ECS CA.

```json
  "CAConnection": {
	"ApiEndpoint":"https://cagw.yourcorp.com/cagw/",
	"CAId":"CA-Jupiter",
	"ClientCertificate":{
		"StoreName":"My",
		"StoreLocation":"LocalMachine",
		"Thumbprint":"e53342b3af95c98884c3438a96eab0e9952fdb6d"
	},
    "TrackingMap":{
		"TrackingInfo":"TrackingInfo",
		"TrackingEmail":"TrackingEmail",
		"TrackingPhone":"TrackingPhone",
		"TrackingName":"TrackingName",
		"TrackingAdditonalEmails":"TrackingAdditonalEmails"
    }
  }
```
## GatewayRegistration
There are no specific Changes for the GatewayRegistration section. Refer to the Refer to the AnyGateway Documentation for more detail.
```json
  "GatewayRegistration": {
    "LogicalName": "EntrustCASandbox",
    "GatewayCertificate": {
      "StoreName": "CA",
      "StoreLocation": "LocalMachine",
      "Thumbprint": "bc6d6b168ce5c08a690c15e03be596bbaa095ebf"
    }
  }
```

## ServiceSettings
There are no specific Changes for the GatewayRegistration section. Refer to the Refer to the AnyGateway Documentation for more detail.
```json
  "ServiceSettings": {
    "ViewIdleMinutes": 8,
    "FullScanPeriodHours": 24,
	"PartialScanPeriodMinutes": 480 
  }
```
