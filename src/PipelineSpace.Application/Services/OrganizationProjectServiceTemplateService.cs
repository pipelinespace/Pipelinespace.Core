using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationProjectServiceTemplateService : IOrganizationProjectServiceTemplateService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly IProjectServiceTemplateRepository _projectServiceTemplateRepository;
        readonly IProgrammingLanguageRepository _programmingLanguageRepository;
        readonly IDataProtectorService _dataProtectorService;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly Func<ConfigurationManagementService, ICMSService> _cmsService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;

        public OrganizationProjectServiceTemplateService(IDomainManagerService domainManagerService,
                                                         IIdentityService identityService,
                                                         IOrganizationRepository organizationRepository,
                                                         IUserRepository userRepository,
                                                         IProjectServiceTemplateRepository projectServiceTemplateRepository,
                                                         IProgrammingLanguageRepository programmingLanguageRepository,
                                                         IDataProtectorService dataProtectorService,
                                                         IEventBusService eventBusService,
                                                         IActivityMonitorService activityMonitorService,
                                                         Func<ConfigurationManagementService, ICMSService> cmsService,
                                                         Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _projectServiceTemplateRepository = projectServiceTemplateRepository;
            _programmingLanguageRepository = programmingLanguageRepository;
            _dataProtectorService = dataProtectorService;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _cmsService = cmsService;
            _cmsCredentialService = cmsCredentialService;
        }

        public async Task CreateOrganizationProjectServiceTemplate(Guid organizationId, OrganizationProjectServiceTemplatePostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to create project service templates in this organization.");
                return;
            }

            OrganizationProjectServiceTemplate existingTemplate = organization.GetProjectServiceTemplateByName(resource.Name);
            if (existingTemplate != null)
            {
                await _domainManagerService.AddConflict($"The project service templates {resource.Name} has already been taken.");
                return;
            }

            ProjectServiceTemplate sourceProjectServiceTemplate = await _projectServiceTemplateRepository.GetProjectServiceTemplateInternalById(resource.SourceProjectServiceTemplateId);
            if (existingTemplate != null)
            {
                await _domainManagerService.AddConflict($"The source project service templates {resource.Name} does not exists.");
                return;
            }

            ProgrammingLanguage programmingLanguage = await _programmingLanguageRepository.GetProgrammingLanguageById(resource.ProgrammingLanguageId);
            if (programmingLanguage == null)
            {
                await _domainManagerService.AddNotFound($"The programming language with id {resource.ProgrammingLanguageId} does not exists.");
                return;
            }

            OrganizationCMS organizationCMS  = organization.GetConfigurationManagementServiceById(resource.ConnectionId);
            if(organizationCMS == null)
            {
                await _domainManagerService.AddNotFound($"The connection with id {resource.ConnectionId} does not exists.");
                return;
            }

            if (organizationCMS.ConnectionType != Domain.Models.Enums.CMSConnectionType.TemplateLevel)
            {
                await _domainManagerService.AddConflict($"The connection with id {resource.ConnectionId} is not for templates.");
                return;
            }

            /*Create repository : BEGIN*/

            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(organizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountId),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountName),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessToken));

            var teamId = string.Empty;
            var projectId = string.Empty;

            if (resource.RepositoryCMSType == ConfigurationManagementService.Bitbucket)
            {
                teamId = resource.TeamId;
                projectId = resource.ProjectExternalId;
            }
            else {
                teamId = resource.ProjectExternalId;
                projectId = resource.ProjectExternalId;
            }

            CMSServiceAvailabilityResultModel cmsServiceAvailability = 
                await _cmsService(organizationCMS.Type).ValidateServiceAvailability(cmsAuthCredential, teamId, projectId, resource.ProjectExternalName, resource.Name);

            if (!cmsServiceAvailability.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsServiceAvailability.GetReasonForNoSuccess()}");
                return;
            }

            //SaveChanges in CMS
            CMSServiceCreateModel serviceCreateModel = CMSServiceCreateModel.Factory.Create(teamId, projectId, resource.ProjectExternalName, resource.Name, true);
            CMSServiceCreateResultModel cmsServiceCreate = await _cmsService(organizationCMS.Type).CreateService(cmsAuthCredential, serviceCreateModel);

            if (!cmsServiceCreate.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsServiceCreate.GetReasonForNoSuccess()}");
                return;
            }

            /*Create repository : END*/
            var template = user.AddProjectTemplateService(organizationId, resource.Name, 
                                                          sourceProjectServiceTemplate.ServiceCMSType, 
                                                          sourceProjectServiceTemplate.ServiceCPSType,
                                                          resource.Description,
                                                          cmsServiceCreate.ServiceExternalUrl, 
                                                          resource.Logo, 
                                                          resource.PipeType,
                                                          Domain.Models.Enums.TemplateType.Standard, 
                                                          Domain.Models.Enums.TemplateAccess.Organization,
                                                          true, resource.ProgrammingLanguageId, resource.Framework, resource.RepositoryCMSType,
                                                          organizationCMS.AccessId,
                                                          organizationCMS.AccessSecret,
                                                          organizationCMS.AccessToken, 
                                                          sourceProjectServiceTemplate.Parameters);

            _projectServiceTemplateRepository.Add(template);
            await _projectServiceTemplateRepository.SaveChanges();

            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var @event = new ProjectServiceTemplateCreatedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                ProjectServiceTemplateId = template.ProjectServiceTemplateId,
                SourceTemplateUrl = sourceProjectServiceTemplate.Url,
                TemplateAccess = template.TemplateAccess,
                NeedCredentials = template.NeedCredentials,
                RepositoryCMSType = template.Credential.CMSType,
                RepositoryAccessId = template.NeedCredentials ? _dataProtectorService.Unprotect(template.Credential.AccessId) : string.Empty,
                RepositoryAccessSecret = template.NeedCredentials ? _dataProtectorService.Unprotect(template.Credential.AccessSecret) : string.Empty,
                RepositoryAccessToken = template.NeedCredentials ? _dataProtectorService.Unprotect(template.Credential.AccessToken) : string.Empty,
                RepositoryUrl = template.Url
            };
            
            await _eventBusService.Publish(queueName: "ProjectServiceTemplateCreatedEvent", @event: @event);
        }

        public async Task UpdateOrganizationProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId, OrganizationProjectServiceTemplatePutRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to create project service templates in this organization.");
                return;
            }

            ProjectServiceTemplate existingTemplate = organization.GetProjectServiceTemplateById(projectServiceTemplateId);
            if (existingTemplate == null)
            {
                await _domainManagerService.AddNotFound($"The project service templated with id {projectServiceTemplateId} does not exists.");
                return;
            }

            existingTemplate.Name = resource.Name;
            existingTemplate.Description = resource.Description;
            existingTemplate.Logo = resource.Logo;

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

        }

        public async Task DeleteOrganizationProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to create project service templates in this organization.");
                return;
            }

            ProjectServiceTemplate existingTemplate = organization.GetProjectServiceTemplateById(projectServiceTemplateId);
            if (existingTemplate == null)
            {
                await _domainManagerService.AddNotFound($"The project service templated with id {projectServiceTemplateId} does not exists.");
                return;
            }

            var isBeingUsed = organization.IsTemplateBeingUsed(projectServiceTemplateId);
            if (isBeingUsed)
            {
                await _domainManagerService.AddConflict($"The template with id {projectServiceTemplateId} is being used by some services. You cannot delete it.");
                return;
            }

            //delete template
            existingTemplate.Delete(loggedUserId);

            //delete relationship
            foreach (var item in existingTemplate.Organizations)
            {
                item.Delete(loggedUserId);
            }
            
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
    }
}
