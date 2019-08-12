using Amazon.CloudFormation;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CPSAWSCredentialService : ICPSCredentialService
    {
        public CPSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateCredentials(string accessId, string accessName, string accessSecret, string accessAppId, string accessAppSecret, string accessDirectory, string accessRegion)
        {
            try
            {
                AmazonCloudFormationClient client = new AmazonCloudFormationClient(accessId, accessSecret, Amazon.RegionEndpoint.GetBySystemName(accessRegion));

                var stacks = await client.ListStacksAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
