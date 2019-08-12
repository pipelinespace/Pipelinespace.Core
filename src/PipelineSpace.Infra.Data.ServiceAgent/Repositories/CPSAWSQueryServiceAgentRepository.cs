using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.CostExplorer;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Infra.Data.ServiceAgent.Helpers;
using PipelineSpace.Infra.Data.ServiceAgent.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CPSAWSQueryServiceAgentRepository : ICPSQueryService
    {
        public async Task<CPSCloudResourceEnvironmentSummaryModel> GetEnvironmentSummary(string organization, string project, string service, string environmentName, string feature, CPSAuthCredentialModel authCredential)
        {
            CPSCloudResourceEnvironmentSummaryModel summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();

            try
            {
                AmazonCloudFormationClient client = new AmazonCloudFormationClient(authCredential.AccessId, authCredential.AccessSecret, Amazon.RegionEndpoint.GetBySystemName(authCredential.AccessRegion));

                string stackName = $"{organization}{project}{service}{environmentName}{feature}".ToLower();
                try
                {
                    var stacksResponse = await client.DescribeStacksAsync(new DescribeStacksRequest() { StackName = stackName });
                    if (stacksResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stack = stacksResponse.Stacks.FirstOrDefault();
                        if (stack != null)
                        {
                            summaryEnvironmentModel = new CPSCloudResourceEnvironmentSummaryModel();
                            summaryEnvironmentModel.Name = environmentName;

                            var statusModel = AWSHelper.GetStatusModel(stack.StackStatus.Value);
                            summaryEnvironmentModel.StatusCode = statusModel.StatusCode;
                            summaryEnvironmentModel.StatusName = statusModel.StatusName;
                            
                            if (stack.Outputs != null)
                            {
                                foreach (var output in stack.Outputs)
                                {
                                    string key = output.OutputKey.First().ToString().ToUpper() + output.OutputKey.Substring(1);
                                    summaryEnvironmentModel.AddProperty(key, output.OutputValue);
                                }
                            }

                            var resources  = await client.DescribeStackResourcesAsync(new DescribeStackResourcesRequest { StackName = stackName });

                            if (resources != null && resources.StackResources != null && resources.StackResources.Any())
                            {
                                foreach (var item in resources.StackResources)
                                {
                                    if (!string.IsNullOrEmpty(item.ResourceType))
                                    {
                                        summaryEnvironmentModel.AddResource(item.ResourceType, item.PhysicalResourceId, item.LogicalResourceId, item.ResourceStatusReason);
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
                AmazonCloudFormationClient client = new AmazonCloudFormationClient(authCredential.AccessId, authCredential.AccessSecret, Amazon.RegionEndpoint.GetBySystemName(authCredential.AccessRegion));

                foreach (var environmentName in environments)
                {
                    string stackName = $"{organization}{project}{service}{environmentName}{feature}".ToLower();
                    try
                    {
                        var stacksResponse = await client.DescribeStacksAsync(new DescribeStacksRequest() { StackName = stackName });
                        if (stacksResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var stack = stacksResponse.Stacks.FirstOrDefault();
                            if (stack != null)
                            {
                                var environment = summaryModel.AddEnvironment(environmentName, stack.StackStatus.Value);
                                if (stack.Outputs != null)
                                {
                                    foreach (var output in stack.Outputs)
                                    {
                                        string key = output.OutputKey.First().ToString().ToUpper() + output.OutputKey.Substring(1);
                                        environment.AddProperty(key, output.OutputValue);
                                    }
                                }
                            }
                            else
                            {
                                summaryModel.AddEnvironment(environmentName, "NO_DEPLOYED_YET");
                            }
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
