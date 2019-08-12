using Newtonsoft.Json;
using System;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSServiceEndpointModel
    {
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("isReady")]
        public bool IsReady { get; set; }
        [JsonProperty("authorization")]
        public CMSVSTSServiceEndPointAuthorizationModel Authorization { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        public static class Factory
        {
            public static CMSVSTSServiceEndpointModel CreateAzureService(Guid id, string name, string subscriptionId, string subscriptionName, string servicePrincipalId, string servicePrincipalKey, string tenantId)
            {
                var serviceEndopint = new CMSVSTSServiceEndpointModel()
                {
                    Id = id,
                    Name = name,
                    Type = "azurerm",
                    Authorization = new CMSVSTSServiceEndPointAuthorizationModel()
                    {
                        Parameters = new CMSVSTSServiceEndPointAuthorizationParameterAzureModel()
                        {
                            AuthenticationType = "spnKey",
                            ServicePrincipalId = servicePrincipalId,
                            ServicePrincipalKey = servicePrincipalKey,
                            TenantId = tenantId
                        },
                        Scheme = "ServicePrincipal"
                    },
                    Url = "https://management.azure.com/",
                    IsReady = true,
                    Data = new
                    {
                        appObjectId = "",
                        azureSpnPermissions = "",
                        azureSpnRoleAssignmentId = "",
                        creationMode = "Manual",
                        environment = "AzureCloud",
                        scopeLevel = "Subscription",
                        spnObjectId = "",
                        subscriptionId = subscriptionId,
                        subscriptionName = subscriptionName
                    }
                };

                return serviceEndopint;
            }

            public static CMSVSTSServiceEndpointModel CreateAWSService(Guid id, string name, string accessKeyId, string secretAccessKey)
            {
                var serviceEndopint = new CMSVSTSServiceEndpointModel()
                {
                    Id = id,
                    Name = name,
                    Type = "AWS",
                    IsReady = true,
                    Authorization = new CMSVSTSServiceEndPointAuthorizationModel()
                    {
                        Scheme = "UsernamePassword",
                        Parameters = new CMSVSTSServiceEndPointAuthorizationParameterAWSModel()
                        {
                            UserName = accessKeyId,
                            Password = secretAccessKey,
                            RoleSessionName = string.Empty,
                            AssumeRoleArn = string.Empty,
                            ExternalId = string.Empty
                        }
                    },
                    Owner = "Library",
                    Url= "https://aws.amazon.com/"
                };

                return serviceEndopint;
            }
        }
    }

    public class CMSVSTSServiceEndPointAuthorizationModel {
        [JsonProperty("parameters")]
        public CMSVSTSServiceEndPointAuthorizationParameterModel Parameters { get; set; }
        [JsonProperty("scheme")]
        public string Scheme { get; set; }
    }

    public class CMSVSTSServiceEndPointAuthorizationParameterModel
    {

    }

    public class CMSVSTSServiceEndPointAuthorizationParameterAzureModel : CMSVSTSServiceEndPointAuthorizationParameterModel
    {
        [JsonProperty("accessTokenFetchingMethod")]
        public string AccessTokenFetchingMethod { get; set; }
        [JsonProperty("accesstoken")]
        public string AccessToken { get; set; }
        [JsonProperty("serviceprincipalid")]
        public string ServicePrincipalId { get; set; }
        [JsonProperty("serviceprincipalkey")]
        public string ServicePrincipalKey { get; set; }
        [JsonProperty("tenantid")]
        public string TenantId { get; set; }
        [JsonProperty("authenticationType")]
        public string AuthenticationType { get; set; }
    }

    public class CMSVSTSServiceEndPointAuthorizationParameterAWSModel : CMSVSTSServiceEndPointAuthorizationParameterModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("assumeRoleArn")]
        public string AssumeRoleArn { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("roleSessionName")]
        public string RoleSessionName { get; set; }
    }
}
