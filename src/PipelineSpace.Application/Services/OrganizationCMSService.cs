using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class OrganizationCMSService : IOrganizationCMSService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IOrganizationCMSRepository _organizationCMSRepository;
        readonly IUserRepository _userRepository;
        readonly IDataProtectorService _dataProtectorService;

        public OrganizationCMSService(IDomainManagerService domainManagerService,
                                      IIdentityService identityService,
                                      IOrganizationRepository organizationRepository,
                                      IOrganizationCMSRepository organizationCMSRepository,
                                      IUserRepository userRepository,
                                      IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _organizationCMSRepository = organizationCMSRepository;
            _userRepository = userRepository;
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateConfigurationManagementService(Guid organizationId, OrganizationCMSPostRp resource)
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
                await _domainManagerService.AddForbidden($"You are not authorized to create settings in this organization.");
                return;
            }

            //OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceByType(resource.Type);
            //if (organizationCMS != null && organizationCMS.ConnectionType == resource.ConnectionType)
            //{
            //    await _domainManagerService.AddConflict($"The configuration management service with type {resource.Type} already exists.");
            //    return;
            //}

            OrganizationCMS existingCMP = organization.GetConfigurationManagementServiceByName(resource.Name);
            if (existingCMP != null)
            {
                await _domainManagerService.AddConflict($"The configuration management service {resource.Name} has already been taken.");
                return;
            }

            //existing same connection in other account
            OrganizationCMS existingInOtherOrganization = await _organizationCMSRepository.FindOrganizationCMSByTypeAndAccountName(resource.Type, resource.AccountName);
            if (existingInOtherOrganization != null)
            {
                await _domainManagerService.AddConflict($"The configuration management service {resource.Type}/{resource.AccountName} has already been taken in other organization.");
                return;
            }

            user.AddConfigurationManagementService(organizationId, 
                                                    resource.Name, 
                                                    resource.Type,
                                                    resource.ConnectionType,
                                                   _dataProtectorService.Protect(resource.AccountId), 
                                                   _dataProtectorService.Protect(resource.AccountName),
                                                   _dataProtectorService.Protect(resource.AccessId),
                                                   _dataProtectorService.Protect(resource.AccessSecret),
                                                   _dataProtectorService.Protect(resource.AccessToken));

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }

        public async Task UpdateConfigurationManagementService(Guid organizationId, Guid organizationCMSId, OrganizationCMSPutRp resource)
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
                await _domainManagerService.AddForbidden($"You are not authorized to update settings in this organization.");
                return;
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (organizationCMS == null)
            {
                await _domainManagerService.AddConflict($"The cloud provider service with id {organizationCMSId} does not exists.");
                return;
            }

            user.UpdateConfigurationManagementService(organizationId, 
                                                      organizationCMSId,
                                                      _dataProtectorService.Protect(resource.AccessId),
                                                      _dataProtectorService.Protect(resource.AccessSecret));

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

        }

        public async Task DeleteConfigurationManagementService(Guid organizationId, Guid organizationCMSId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to delete settings in this organization.");
                return;
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(organizationCMSId);
            if (organizationCMS == null)
            {
                await _domainManagerService.AddConflict($"The configuration management service with id {organizationCMSId} does not exists.");
                return;
            }

            List<Project> relatedProjects = organization.GetProjectsByCMSId(organizationCMS.OrganizationCMSId);
            if (relatedProjects.Any())
            {
                await _domainManagerService.AddConflict($"There are projects already configured with the configuration management service {organizationCMS.Name}.");
                return;
            }

            user.DeleteConfigurationManagementService(organizationId, organizationCMSId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

        }
    }
}
