using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class ProjectFeatureBitbucketHandlerService : IProjectFeatureHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        private const string API_VERSION = "2.0";
        private const string API_URL = "https://api.bitbucket.org";
        readonly Func<CloudProviderService, ICPSService> _cpsService;

        public ProjectFeatureBitbucketHandlerService(IOptions<VSTSServiceOptions> vstsOptions,
            Func<CloudProviderService, ICPSService> cpsService)
        {
            _vstsOptions = vstsOptions;
            _cpsService = cpsService;
        }

        public async Task CompleteProjectFeature(ProjectFeatureCompletedEvent @event)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", @event.CMSAccountId, @event.CMSAccessSecret))));
            client.BaseAddress = new Uri(API_URL);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            var defaultTeam = teamResult.Teams.FirstOrDefault(c => c.TeamId.Equals(@event.OrganizationExternalId));
            
            foreach (var item in @event.Services)
            {
                /*Delete Infrastructure*/
                if (@event.DeleteInfrastructure)
                {
                    CPSAuthModel authModel = new CPSAuthModel();
                    authModel.AccessId = @event.CPSAccessId;
                    authModel.AccessName = @event.CPSAccessName;
                    authModel.AccessSecret = @event.CPSAccessSecret;
                    authModel.AccessRegion = @event.CPSAccessRegion;
                    authModel.AccessAppId = @event.CPSAccessAppId;
                    authModel.AccessAppSecret = @event.CPSAccessAppSecret;
                    authModel.AccessDirectory = @event.CPSAccessDirectory;

                    string cloudServiceName = $"{@event.OrganizationName}{@event.ProjectName}{item.ServiceName}development{@event.FeatureName}".ToLower();
                    await _cpsService(@event.CPSType).DeleteService(cloudServiceName, authModel);
                }

                var pullRequestModel = new
                {
                    title = $"feature {@event.FeatureName}",
                    //Description = $"The feature {@event.FeatureName} requests merge operation",
                    source = new {
                        branch = new {
                            name = @event.FeatureName.ToLower()
                        }
                    },
                    destination = new {
                        branch = new
                        {
                            name = "master"
                        }
                    }
                };
                
                response = await client.PostAsync($"/{API_VERSION}/repositories/{defaultTeam.UserName}/{item.ServiceExternalId}/pullrequests", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(pullRequestModel), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
            }

        }

    }
}
