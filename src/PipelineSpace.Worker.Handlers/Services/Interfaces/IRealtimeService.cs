using PipelineSpace.Worker.Handlers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface IRealtimeService
    {
        Task NotifyProjectStatus(string userId, Guid organizationId, Guid projectId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials);
        Task NotifyProjectPipeStatus(string userId, Guid organizationId, Guid projectId, Guid projectServiceId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials);
        Task NotifyFeaturePipeStatus(string userId, Guid organizationId, Guid projectId, Guid featureId, Guid projectServiceId, string activityName, string status, HttpClientWrapperAuthorizationModel internalAuthCredentials);

    }
}
