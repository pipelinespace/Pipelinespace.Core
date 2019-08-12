using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Infra.Data.ServiceAgent.Helpers;
using PipelineSpace.Infra.Data.ServiceAgent.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Billing;
using Microsoft.Azure.Management.Billing.Models;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CPSAzureQueryServiceAgentRepository : ICPSQueryService
    {
        
        public async Task<CPSCloudResourceEnvironmentSummaryModel> GetEnvironmentSummary(string organization, string project, string service, string environmentName, string feature, CPSAuthCredentialModel authCredential)
        {
            CPSCloudResourceEnvironmentSummaryModel summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();

            try
            {
                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(authCredential.AccessDirectory, authCredential.AccessAppId, authCredential.AccessAppSecret);
                ResourceManagementClient resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = authCredential.AccessId;

                string resourceGroupName = $"{organization}{project}{service}{environmentName}{feature}".ToLower();
                try
                {
                    
                    var resourceGroupResponse = resourceManagementClient.ResourceGroups.Get(resourceGroupName);
                    if (resourceGroupResponse != null)
                    {
                        summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();
                        summaryEnvironmentModel.Name = environmentName;
                        
                        var deployments = resourceManagementClient.Deployments.ListByResourceGroup(resourceGroupName, new Microsoft.Rest.Azure.OData.ODataQuery<Microsoft.Azure.Management.ResourceManager.Models.DeploymentExtendedFilter>() { Top = 1, OrderBy = "Name desc" });
                        DeploymentExtended deployment = null;
                        foreach (var item in deployments)
                        {
                            deployment = item;
                            break;
                        }

                        var statusModel = AzureHelper.GetStatusModel(deployment.Properties.ProvisioningState);
                        summaryEnvironmentModel.StatusCode = statusModel.StatusCode;
                        summaryEnvironmentModel.StatusName = statusModel.StatusName;

                        if (deployment != null && deployment.Properties.Outputs != null)
                        {
                            var outPuts = JObject.Parse(JsonConvert.SerializeObject(deployment.Properties.Outputs));
                            foreach (var output in outPuts)
                            {
                                string key = output.Key.First().ToString().ToUpper() + output.Key.Substring(1);
                                summaryEnvironmentModel.AddProperty(key, output.Value["value"].ToString());
                            }
                        }

                        var resources  = resourceManagementClient.Resources.ListByResourceGroup(resourceGroupName);

                        if (resources != null && resources.Any()) {
                            foreach (var item in resources)
                            {
                                if (!string.IsNullOrEmpty(item.Kind)) {
                                    summaryEnvironmentModel.AddResource(item.Type, item.Id, item.Name, statusModel.StatusName);
                                }
                                
                            }
                        }

                    }
                    else
                    {
                        summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();
                        summaryEnvironmentModel.Name = environmentName;
                        summaryEnvironmentModel.StatusCode = PipelineEnvironmentStatusEnumModel.Pending.ToString();
                        summaryEnvironmentModel.StatusName = PipelineEnvironmentStatusEnumModel.Pending.GetDescription();
                    }
                }
                catch (System.Exception ex)
                {
                    summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();
                    summaryEnvironmentModel.Name = environmentName;
                    summaryEnvironmentModel.StatusCode = PipelineEnvironmentStatusEnumModel.Pending.ToString();
                    summaryEnvironmentModel.StatusName = PipelineEnvironmentStatusEnumModel.Pending.GetDescription();
                }

            }
            catch (System.Exception)
            {

            }

            return summaryEnvironmentModel;
        }
        
        public async Task<CPSCloudResourceSummaryModel> GetSummary(string organization, string project, string service, List<string> environments, string feature, CPSAuthCredentialModel authCredential)
        {
            CPSCloudResourceSummaryModel summaryModel = new CPSCloudResourceSummaryModel();

            try
            {
                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(authCredential.AccessDirectory, authCredential.AccessAppId, authCredential.AccessAppSecret);
                ResourceManagementClient resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = authCredential.AccessId;

                foreach (var environmentName in environments)
                {
                    string resourceGroupName = $"{organization}{project}{service}{environmentName}{feature}".ToLower();
                    try
                    {
                        var resourceGroupResponse = resourceManagementClient.ResourceGroups.Get(resourceGroupName);
                        if(resourceGroupResponse != null)
                        {
                            var environment = summaryModel.AddEnvironment(environmentName, resourceGroupResponse.Properties.ProvisioningState);
                            var deployments = resourceManagementClient.Deployments.ListByResourceGroup(resourceGroupName, new Microsoft.Rest.Azure.OData.ODataQuery<Microsoft.Azure.Management.ResourceManager.Models.DeploymentExtendedFilter>() { Top = 1, OrderBy = "Name desc" });
                            DeploymentExtended deployment = null;
                            foreach (var item in deployments)
                            {
                                deployment = item;
                                break;
                            }
                            if (deployment!= null && deployment.Properties.Outputs != null)
                            {
                                var outPuts = JObject.Parse(JsonConvert.SerializeObject(deployment.Properties.Outputs));
                                foreach (var output in outPuts)
                                {
                                    string key = output.Key.First().ToString().ToUpper() + output.Key.Substring(1);
                                    environment.AddProperty(key, output.Value["value"].ToString());
                                }
                            }
                        }
                        else
                        {
                            summaryModel.AddEnvironment(environmentName, "NO_DEPLOYED_YET");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        summaryModel.AddEnvironment(environmentName, "NO_DEPLOYED_YET");
                    }

                }

            }
            catch (System.Exception)
            {

            }

            return summaryModel;
        }
    }
}