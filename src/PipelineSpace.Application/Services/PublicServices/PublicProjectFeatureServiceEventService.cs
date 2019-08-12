using Newtonsoft.Json;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.PublicServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.CrossCutting.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModels = PipelineSpace.Domain.Models;

namespace PipelineSpace.Application.Services.PublicServices
{
    public class PublicProjectFeatureServiceEventService : IPublicProjectFeatureServiceEventService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;

        public PublicProjectFeatureServiceEventService(IDomainManagerService domainManagerService, 
                                                       IIdentityService identityService, 
                                                       IOrganizationRepository organizationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
        }

        public async Task CreateProjectFeatureServiceEvent(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, ProjectFeatureServiceEventPostRp resource)
        {
            DomainModels.Organization organization = await _organizationRepository.GetOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            /*PIPELINE STATUS###########################################################################################################*/
            /*##########################################################################################################################*/
            //update pipeline status when build
            string eventStatus = string.Empty;
            DateTime eventDate = DateTime.UtcNow;
            BaseEventType baseEventType = BaseEventType.Build;

            //update pipeline status when build
            if (resource.GetEventType() == PipelineEventType.BuildStarted)
            {
                eventStatus = "Queued";
                baseEventType = BaseEventType.Build;

                if (eventDate > featureService.LastBuildEventDate)
                {
                    featureService.PipelineStatus = PipelineStatus.Building;
                    featureService.LastPipelineBuildStatus = PipelineBuildStatus.Building;

                    featureService.LastBuildEventDate = eventDate;
                }
            }

            if (resource.GetEventType() == PipelineEventType.BuildCompleted)
            {
                var buildModel = resource.BuildBuildModel();
                eventStatus = buildModel.Status.FirstCharToUpper();
                eventDate = buildModel.FinishTime;
                baseEventType = BaseEventType.Build;

                featureService.LastBuildVersionId = buildModel.Id.ToString();
                featureService.LastBuildVersionName = buildModel.BuildNumber;

                if (eventDate > featureService.LastBuildEventDate)
                {
                    if (buildModel.Status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.BuildSucceeded;
                        featureService.LastPipelineBuildStatus = PipelineBuildStatus.BuildSucceeded;

                        featureService.LastBuildSuccessVersionId = buildModel.Id.ToString();
                        featureService.LastBuildSuccessVersionName = buildModel.BuildNumber;
                    }

                    if (buildModel.Status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                        buildModel.Status.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.BuildFailed;
                        featureService.LastPipelineBuildStatus = PipelineBuildStatus.BuildFailed;
                    }

                    if (buildModel.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.BuildCanceled;
                        featureService.LastPipelineBuildStatus = PipelineBuildStatus.BuildCanceled;
                    }

                    featureService.LastBuildEventDate = eventDate;
                }

                //Delivery
                featureService.AddDeliveryBuildCompleted(buildModel.Id, buildModel.BuildNumber, eventStatus, eventDate);
            }

            //update pipeline status when release
            if (resource.GetEventType() == PipelineEventType.ReleaseStarted)
            {
                var releaseModel = resource.BuildReleaseStartedModel();
                eventStatus = releaseModel.Environment.Status.FirstCharToUpper();
                eventDate = releaseModel.Release.CreatedOn;
                baseEventType = BaseEventType.Release;

                var environment = featureService.Environments.FirstOrDefault(x => x.ProjectFeatureEnvironment.Name.Equals(releaseModel.Environment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = releaseModel.Environment.Status.FirstCharToUpper();
                    environment.LastStatusCode = PipelineReleaseStatus.Deploying.ToString();
                    environment.LastEventDate = eventDate;
                }

                if (eventDate > featureService.LasReleaseEventDate)
                {
                    featureService.PipelineStatus = PipelineStatus.Deploying;
                    featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.Deploying;

                    featureService.LasReleaseEventDate = eventDate;
                }

                //Delivery
                featureService.AddDeliveryReleaseStarted(int.Parse(releaseModel.VersionId),
                                                         releaseModel.VersionName,
                                                         releaseModel.Release.Environments.Select(x => new Domain.ModelUtility.DeliveryEnvironmentModel() { Name = x.Name, Rank = x.Rank, Status = x.Status.FirstCharToUpper() }).ToList());

            }

            if (resource.GetEventType() == PipelineEventType.ReleasePendingApproval)
            {
                var releaseApprovalModel = resource.BuildReleaseApprovalModel();
                eventStatus = "Release approval pending";
                eventDate = releaseApprovalModel.Approval.CreatedOn;
                baseEventType = BaseEventType.Release;

                var environment = featureService.Environments.FirstOrDefault(x => x.ProjectFeatureEnvironment.Name.Equals(releaseApprovalModel.Approval.ReleaseEnvironment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = "Release approval pending";
                    environment.LastStatusCode = PipelineReleaseStatus.DeployPendingApproval.ToString();
                    environment.LastApprovalId = releaseApprovalModel.Approval.Id.ToString();
                    environment.LastEventDate = eventDate;
                }

                if (eventDate > featureService.LasReleaseEventDate)
                {
                    featureService.PipelineStatus = PipelineStatus.DeployPendingApproval;
                    featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployPendingApproval;

                    featureService.LasReleaseEventDate = eventDate;
                }

                //Delivery
                featureService.UpdateDeliveryReleaseStatus(int.Parse(releaseApprovalModel.VersionId), releaseApprovalModel.VersionName, releaseApprovalModel.Approval.ReleaseEnvironment.Name, eventStatus);
            }

            if (resource.GetEventType() == PipelineEventType.ReleaseCompletedApproval)
            {
                var releaseApprovalModel = resource.BuildReleaseApprovalModel();
                eventStatus = $"Release approval {releaseApprovalModel.Approval.Status}";
                eventDate = releaseApprovalModel.Approval.CreatedOn;
                baseEventType = BaseEventType.Release;

                var environment = featureService.Environments.FirstOrDefault(x => x.ProjectFeatureEnvironment.Name.Equals(releaseApprovalModel.Approval.ReleaseEnvironment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = $"Release approval {releaseApprovalModel.Approval.Status}";
                    environment.LastStatusCode = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineReleaseStatus.DeployRejectedApproval.ToString() : PipelineReleaseStatus.DeployAcceptedApproval.ToString();
                    environment.LastEventDate = eventDate;
                }

                if (eventDate > featureService.LasReleaseEventDate)
                {
                    featureService.PipelineStatus = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineStatus.DeployRejectedApproval : PipelineStatus.DeployAcceptedApproval;
                    featureService.LastPipelineReleaseStatus = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineReleaseStatus.DeployRejectedApproval : PipelineReleaseStatus.DeployAcceptedApproval;

                    featureService.LasReleaseEventDate = eventDate;
                }

                //Delivery
                featureService.UpdateDeliveryReleaseStatus(int.Parse(releaseApprovalModel.VersionId), releaseApprovalModel.VersionName, releaseApprovalModel.Approval.ReleaseEnvironment.Name, eventStatus);
            }

            if (resource.GetEventType() == PipelineEventType.ReleaseCompleted)
            {
                var releaseModel = resource.BuildReleaseModel();
                eventStatus = releaseModel.Environment.Status.FirstCharToUpper();
                eventDate = releaseModel.Deployment.CompletedOn;
                baseEventType = BaseEventType.Release;

                if (releaseModel.Environment.Status.Equals("Rejected", StringComparison.InvariantCultureIgnoreCase))
                {
                    eventDate = releaseModel.Deployment.LastModifiedOn;
                }

                var environment = featureService.Environments.FirstOrDefault(x => x.ProjectFeatureEnvironment.Name.Equals(releaseModel.Environment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = releaseModel.Environment.Status.FirstCharToUpper();
                    environment.LastVersionId = releaseModel.Deployment.VersionId;
                    environment.LastVersionName = releaseModel.Deployment.VersionName;
                    environment.LastEventDate = eventDate;
                }

                if (releaseModel.Environment.Status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                {
                    environment.LastSuccessVersionId = releaseModel.Deployment.VersionId;
                    environment.LastSuccessVersionName = releaseModel.Deployment.VersionName;
                    environment.LastStatusCode = PipelineReleaseStatus.DeploySucceeded.ToString();
                }

                if (releaseModel.Environment.Status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                    releaseModel.Environment.Status.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                {
                    environment.LastStatusCode = PipelineReleaseStatus.DeployFailed.ToString();
                }

                if (releaseModel.Environment.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                {
                    environment.LastStatusCode = PipelineReleaseStatus.DeployCanceled.ToString();
                }

                if (releaseModel.Environment.Status.Equals("Rejected", StringComparison.InvariantCultureIgnoreCase))
                {
                    environment.LastStatusCode = PipelineReleaseStatus.DeployRejected.ToString();
                }

                if (eventDate > featureService.LasReleaseEventDate)
                {
                    if (releaseModel.Environment.Status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.DeploySucceeded;
                        featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.DeploySucceeded;
                    }

                    if (releaseModel.Environment.Status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                        releaseModel.Environment.Status.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.DeployFailed;
                        featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployFailed;
                    }

                    if (releaseModel.Environment.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.DeployCanceled;
                        featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployCanceled;
                    }

                    if (releaseModel.Environment.Status.Equals("Rejected", StringComparison.InvariantCultureIgnoreCase))
                    {
                        featureService.PipelineStatus = PipelineStatus.DeployRejected;
                        featureService.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployRejected;
                    }

                    featureService.LasReleaseEventDate = eventDate;
                }

                //Delivery
                featureService.UpdateDeliveryReleaseStatus(int.Parse(releaseModel.Deployment.VersionId), releaseModel.Deployment.VersionName, releaseModel.Environment.Name, eventStatus);

            }

            /*SERVICE STATUS###########################################################################################################*/
            /*#########################################################################################################################*/
            //activate the service when build is failed 
            if (resource.GetEventType() == PipelineEventType.BuildCompleted &&
               (eventStatus.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                eventStatus.Equals("Error", StringComparison.InvariantCultureIgnoreCase) ||
                eventStatus.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase)) &&
                featureService.Status == EntityStatus.Preparing)
            {
                featureService.Status = EntityStatus.Active;
            }

            //activate the service when realease is completed (in any event status)
            if (resource.GetEventType() == PipelineEventType.ReleaseCompleted && featureService.Status == EntityStatus.Preparing)
            {
                featureService.Status = EntityStatus.Active;
            }

            featureService.AddEvent(baseEventType, resource.GetEventType().GetDescription(), resource.Message.Text, eventStatus, JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.Resource), eventDate);

            //check if any feature service is in preparing status yet
            var preparingServices = feature.GetPreparingServices();
            if (!preparingServices.Any())
            {
                feature.Status = EntityStatus.Active;
            }
            
            _organizationRepository.Update(organization);

            await _organizationRepository.SaveChanges();
        }
    }
}
