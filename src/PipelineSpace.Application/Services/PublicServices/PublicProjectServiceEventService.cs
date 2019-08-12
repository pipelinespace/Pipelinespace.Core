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
    public class PublicProjectServiceEventService : IPublicProjectServiceEventService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;

        public PublicProjectServiceEventService(IDomainManagerService domainManagerService, 
                                                IIdentityService identityService, 
                                                IOrganizationRepository organizationRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
        }

        public async Task CreateProjectServiceEvent(Guid organizationId, Guid projectId, Guid serviceId, ProjectServiceEventPostRp resource)
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

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }

            /*PIPELINE STATUS###########################################################################################################*/
            /*##########################################################################################################################*/
            string eventStatus = string.Empty;
            DateTime eventDate = DateTime.UtcNow;
            BaseEventType baseEventType = BaseEventType.Build;

            //update pipeline status when build
            if (resource.GetEventType() == PipelineEventType.BuildStarted)
            {
                baseEventType = BaseEventType.Build;
                eventStatus = "Queued";

                if(eventDate > service.LastBuildEventDate)
                {
                    service.PipelineStatus = PipelineStatus.Building;
                    service.LastPipelineBuildStatus = PipelineBuildStatus.Building;

                    service.LastBuildEventDate = eventDate;
                }    
            }

            if (resource.GetEventType() == PipelineEventType.BuildCompleted)
            {
                var buildModel = resource.BuildBuildModel();
                eventStatus = buildModel.Status.FirstCharToUpper();
                eventDate = buildModel.FinishTime;
                baseEventType = BaseEventType.Build;

                service.LastBuildVersionId = buildModel.Id.ToString();
                service.LastBuildVersionName = buildModel.BuildNumber;

                if (eventDate > service.LastBuildEventDate)
                {
                    if (buildModel.Status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.BuildSucceeded;
                        service.LastPipelineBuildStatus = PipelineBuildStatus.BuildSucceeded;

                        service.LastBuildSuccessVersionId = buildModel.Id.ToString();
                        service.LastBuildSuccessVersionName = buildModel.BuildNumber;
                    }

                    if (buildModel.Status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                        buildModel.Status.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.BuildFailed;
                        service.LastPipelineBuildStatus = PipelineBuildStatus.BuildFailed;
                    }

                    if (buildModel.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.BuildCanceled;
                        service.LastPipelineBuildStatus = PipelineBuildStatus.BuildCanceled;
                    }

                    service.LastBuildEventDate = eventDate;
                }

                //Delivery
                service.AddDeliveryBuildCompleted(buildModel.Id, buildModel.BuildNumber, eventStatus, eventDate);
            }

            //update pipeline status when release
            if (resource.GetEventType() == PipelineEventType.ReleaseStarted)
            {
                var releaseModel = resource.BuildReleaseStartedModel();
                eventStatus = releaseModel.Environment.Status.FirstCharToUpper();
                eventDate = releaseModel.Release.CreatedOn;
                baseEventType = BaseEventType.Release;

                var environment = service.Environments.FirstOrDefault(x => x.ProjectEnvironment.Name.Equals(releaseModel.Environment.Name, StringComparison.InvariantCultureIgnoreCase));
                if(environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = releaseModel.Environment.Status.FirstCharToUpper();
                    environment.LastStatusCode = PipelineReleaseStatus.Deploying.ToString();
                    environment.LastEventDate = eventDate;
                }
                
                if (eventDate > service.LasReleaseEventDate)
                {
                    service.PipelineStatus = PipelineStatus.Deploying;
                    service.LastPipelineReleaseStatus = PipelineReleaseStatus.Deploying;

                    service.LasReleaseEventDate = eventDate;
                }

                //Delivery
                service.AddDeliveryReleaseStarted(int.Parse(releaseModel.VersionId), 
                                                  releaseModel.VersionName, 
                                                  releaseModel.Release.Environments.Select(x=> new Domain.ModelUtility.DeliveryEnvironmentModel() { Name = x.Name, Rank = x.Rank, Status = x.Status.FirstCharToUpper() }).ToList());
            }

            if (resource.GetEventType() == PipelineEventType.ReleasePendingApproval)
            {
                var releaseApprovalModel = resource.BuildReleaseApprovalModel();
                eventStatus = "Release approval pending";
                eventDate = releaseApprovalModel.Approval.CreatedOn;
                baseEventType = BaseEventType.Release;

                var environment = service.Environments.FirstOrDefault(x => x.ProjectEnvironment.Name.Equals(releaseApprovalModel.Approval.ReleaseEnvironment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = "Release approval pending";
                    environment.LastStatusCode = PipelineReleaseStatus.DeployPendingApproval.ToString();
                    environment.LastApprovalId = releaseApprovalModel.Approval.Id.ToString();
                    environment.LastEventDate = eventDate;
                }

                if (eventDate > service.LasReleaseEventDate)
                {
                    service.PipelineStatus = PipelineStatus.DeployPendingApproval;
                    service.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployPendingApproval;

                    service.LasReleaseEventDate = eventDate;
                }

                //Delivery
                service.UpdateDeliveryReleaseStatus(int.Parse(releaseApprovalModel.VersionId), releaseApprovalModel.VersionName, releaseApprovalModel.Approval.ReleaseEnvironment.Name, eventStatus);
            }

            if (resource.GetEventType() == PipelineEventType.ReleaseCompletedApproval)
            {
                var releaseApprovalModel = resource.BuildReleaseApprovalModel();
                eventStatus = $"Release approval {releaseApprovalModel.Approval.Status}";
                eventDate = releaseApprovalModel.Approval.ModifiedOn;
                baseEventType = BaseEventType.Release;

                var environment = service.Environments.FirstOrDefault(x => x.ProjectEnvironment.Name.Equals(releaseApprovalModel.Approval.ReleaseEnvironment.Name, StringComparison.InvariantCultureIgnoreCase));
                if (environment.LastEventDate < eventDate)
                {
                    environment.LastStatus = $"Release approval {releaseApprovalModel.Approval.Status}";
                    environment.LastStatusCode = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineReleaseStatus.DeployRejectedApproval.ToString() : PipelineReleaseStatus.DeployAcceptedApproval.ToString();
                    environment.LastEventDate = eventDate;
                }
                
                if (eventDate > service.LasReleaseEventDate)
                {
                    service.PipelineStatus = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineStatus.DeployRejectedApproval : PipelineStatus.DeployAcceptedApproval;
                    service.LastPipelineReleaseStatus = releaseApprovalModel.Approval.Status.Equals("rejected", StringComparison.InvariantCultureIgnoreCase) ? PipelineReleaseStatus.DeployRejectedApproval : PipelineReleaseStatus.DeployAcceptedApproval;

                    service.LasReleaseEventDate = eventDate;
                }

                //Delivery
                service.UpdateDeliveryReleaseStatus(int.Parse(releaseApprovalModel.VersionId), releaseApprovalModel.VersionName, releaseApprovalModel.Approval.ReleaseEnvironment.Name, eventStatus);
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

                var environment = service.Environments.FirstOrDefault(x => x.ProjectEnvironment.Name.Equals(releaseModel.Environment.Name, StringComparison.InvariantCultureIgnoreCase));
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

                if (eventDate > service.LasReleaseEventDate)
                {  
                    if (releaseModel.Environment.Status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.DeploySucceeded;
                        service.LastPipelineReleaseStatus = PipelineReleaseStatus.DeploySucceeded;
                    }

                    if (releaseModel.Environment.Status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                        releaseModel.Environment.Status.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.DeployFailed;
                        service.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployFailed;
                    }

                    if (releaseModel.Environment.Status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.DeployCanceled;
                        service.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployCanceled;
                    }

                    if (releaseModel.Environment.Status.Equals("Rejected", StringComparison.InvariantCultureIgnoreCase))
                    {
                        service.PipelineStatus = PipelineStatus.DeployRejected;
                        service.LastPipelineReleaseStatus = PipelineReleaseStatus.DeployRejected;
                    }
                    service.LasReleaseEventDate = eventDate;
                }

                //Delivery
                service.UpdateDeliveryReleaseStatus(int.Parse(releaseModel.Deployment.VersionId), releaseModel.Deployment.VersionName, releaseModel.Environment.Name, eventStatus);
            }

            /*SERVICE STATUS###########################################################################################################*/
            /*#########################################################################################################################*/
            //activate the service when build is failed 
            if (resource.GetEventType() == PipelineEventType.BuildCompleted &&
                (eventStatus.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) ||
                 eventStatus.Equals("Error", StringComparison.InvariantCultureIgnoreCase) ||
                 eventStatus.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase)) &&
                 service.Status == EntityStatus.Preparing)
            {
                service.Status = EntityStatus.Active;
            }

            //activate the service when realease is completed (in any event status)
            if (resource.GetEventType() == PipelineEventType.ReleaseCompleted  && service.Status == EntityStatus.Preparing)
            {
                service.Status = EntityStatus.Active;
            }

            service.AddEvent(baseEventType, resource.GetEventType().GetDescription(), resource.Message.Text, eventStatus, JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.DetailedMessage), JsonConvert.SerializeObject(resource.Resource), eventDate);

            _organizationRepository.Update(organization);

            await _organizationRepository.SaveChanges();
        }
    }
}
