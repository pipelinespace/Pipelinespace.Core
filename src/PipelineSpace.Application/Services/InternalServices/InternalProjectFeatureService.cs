using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices
{
    public class InternalProjectFeatureService : IInternalProjectFeatureService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureRepository _projectFeatureRepository;

        public InternalProjectFeatureService(IDomainManagerService domainManagerService,
                                             IProjectFeatureRepository projectFeatureRepository)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureRepository = projectFeatureRepository;
        }

        public async Task ActivateProjectFeature(Guid organizationId, Guid projectId, Guid featureId)
        {
            var feature = await _projectFeatureRepository.GetProjectFeatureById(organizationId, projectId, featureId);

            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != EntityStatus.Preparing)
            {
                await _domainManagerService.AddConflict($"The feature with id {featureId} must be in status NEW to be activated.");
                return;
            }

            feature.Activate();

            _projectFeatureRepository.Update(feature);

            await _projectFeatureRepository.SaveChanges();
        }

        public async Task PatchProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, ProjectFeatureServicePatchtRp resource)
        {
            var feature = await _projectFeatureRepository.GetProjectFeatureById(organizationId, projectId, featureId);

            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The feature with id {featureId} does not exists.");
                return;
            }

            var featureService = feature.GetFeatureServiceById(serviceId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The feature pipe with id {serviceId} does not exists.");
                return;
            }

            if (resource.CommitStageId.HasValue)
            {
                featureService.CommitStageId = resource.CommitStageId.Value;
            }

            if (resource.ReleaseStageId.HasValue)
            {
                featureService.ReleaseStageId = resource.ReleaseStageId.Value;
            }

            if (resource.CommitServiceHookId.HasValue)
            {
                featureService.CommitServiceHookId = resource.CommitServiceHookId.Value;
            }

            if (resource.ReleaseServiceHookId.HasValue)
            {
                featureService.ReleaseServiceHookId = resource.ReleaseServiceHookId.Value;
            }

            if (resource.CodeServiceHookId.HasValue)
            {
                featureService.CodeServiceHookId = resource.CodeServiceHookId.Value;
            }

            if (resource.ReleaseStartedServiceHookId.HasValue)
            {
                featureService.ReleaseStartedServiceHookId = resource.ReleaseStartedServiceHookId.Value;
            }

            if (resource.ReleasePendingApprovalServiceHookId.HasValue)
            {
                featureService.ReleasePendingApprovalServiceHookId = resource.ReleasePendingApprovalServiceHookId.Value;
            }

            if (resource.ReleaseCompletedApprovalServiceHookId.HasValue)
            {
                featureService.ReleaseCompletedApprovalServiceHookId = resource.ReleaseCompletedApprovalServiceHookId.Value;
            }

            if (resource.PipelineStatus.HasValue)
            {
                featureService.PipelineStatus = resource.PipelineStatus.Value;
            }

            _projectFeatureRepository.Update(feature);
            await _projectFeatureRepository.SaveChanges();
        }
    }
}
