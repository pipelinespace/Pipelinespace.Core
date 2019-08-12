using Microsoft.Extensions.Options;
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
    public class ProjectServiceBitbucketHandlerService : IProjectServiceHandlerService
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        private const string API_VERSION = "2.0";
        private const string API_URL = "https://api.bitbucket.org";

        public ProjectServiceBitbucketHandlerService(IOptions<VSTSServiceOptions> vstsOptions)
        {
            _vstsOptions = vstsOptions;
        }

        public async Task DeleteRepository(ProjectServiceDeletedEvent @event)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", @event.CMSAccountId, @event.CMSAccessSecret))));
            client.BaseAddress = new Uri(API_URL);

            var response = await client.GetAsync($"/{API_VERSION}/teams?role=admin");

            var teamResult = await response.MapTo<CMSBitBucketTeamListModel>();

            var defaultTeam = teamResult.Teams.FirstOrDefault(c => c.TeamId.Equals(@event.OrganizationExternalId));

            response = await client.DeleteAsync($"/{API_VERSION}/repositories/{defaultTeam.UserName}/{@event.ProjectServiceExternalId}");

            response.EnsureSuccessStatusCode();
        }
    }
}
