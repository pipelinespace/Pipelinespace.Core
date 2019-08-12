using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest.Azure.Authentication;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using PipelineSpace.Worker.Monitor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class CPSAzureService : ICPSService
    {
        public async Task DeleteService(string name, CPSAuthModel auth)
        {
            try
            {
                var serviceCreds = await ApplicationTokenProvider.LoginSilentAsync(auth.AccessDirectory, auth.AccessAppId, auth.AccessAppSecret);
                ResourceManagementClient resourceManagementClient = new ResourceManagementClient(serviceCreds);
                resourceManagementClient.SubscriptionId = auth.AccessId;

                await resourceManagementClient.ResourceGroups.BeginDeleteAsync(name.ToLower());
            }
            catch (Exception ex)
            {
                TelemetryClientManager.Instance.TrackException(ex);

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
