using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest.Azure.Authentication;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CPSAzureCredentialService : ICPSCredentialService
    {
        public CPSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateCredentials(string accessId, string accessName, string accessSecret, string accessAppId, string accessAppSecret, string accessDirectory, string accessRegion)
        {
            try
            {
                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(accessDirectory, accessAppId, accessAppSecret);
                ResourceManagementClient resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = accessId;

                var resourceGroupResponse = await resourceManagementClient.ResourceGroups.ListAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
