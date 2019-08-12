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
    public class OrganizationCPSService : IOrganizationCPSService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IOrganizationCPSRepository _organizationCPSRepository;
        readonly IUserRepository _userRepository;
        readonly Func<CloudProviderService, ICPSCredentialService> _cpsCredentialService;
        readonly IDataProtectorService _dataProtectorService;

        public OrganizationCPSService(IDomainManagerService domainManagerService, 
                                      IIdentityService identityService, 
                                      IOrganizationRepository organizationRepository, 
                                      IOrganizationCPSRepository organizationCPSRepository,
                                      IUserRepository userRepository,
                                      Func<CloudProviderService, ICPSCredentialService> cpsCredentialService,
                                      IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _organizationCPSRepository = organizationCPSRepository;
            _userRepository = userRepository;
            _cpsCredentialService = cpsCredentialService;
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateCloudProviderService(Guid organizationId, OrganizationCPSPostRp resource)
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

            //OrganizationCPS organizationCPS = organization.GetCloudProviderServiceByType(resource.Type);
            //if (organizationCPS != null)
            //{
            //    await _domainManagerService.AddConflict($"The cloud provider service with type {resource.Type} already exists.");
            //    return;
            //}

            OrganizationCPS existingCSP = organization.GetCloudProviderServiceByName(resource.Name);
            if (existingCSP != null)
            {
                await _domainManagerService.AddConflict($"The cloud provider service {resource.Name} has already been taken.");
                return;
            }

            bool validCredentials = await _cpsCredentialService(resource.Type).ValidateCredentials(resource.AccessId, resource.AccessName, resource.AccessSecret, resource.AccessAppId,
                                                                                                   resource.AccessAppSecret, resource.AccessDirectory, resource.AccessRegion);

            if (!validCredentials)
            {
                if(resource.Type == CloudProviderService.AWS)
                {
                    await _domainManagerService.AddConflict($"The credentials are not valid");
                }
                else
                {
                    await _domainManagerService.AddConflict($"The credentials are not valid or the client does not have enough privileges");
                }
                return;
            }

            user.AddCloudProviderService(organizationId, 
                                         resource.Name, 
                                         resource.Type, 
                                         _dataProtectorService.Protect(resource.AccessId), 
                                         _dataProtectorService.Protect(resource.AccessName), 
                                         _dataProtectorService.Protect(resource.AccessSecret),
                                         _dataProtectorService.Protect(resource.AccessAppId),
                                         _dataProtectorService.Protect(resource.AccessAppSecret),
                                         _dataProtectorService.Protect(resource.AccessDirectory),
                                         _dataProtectorService.Protect(resource.AccessRegion));

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

        }

        public async Task UpdateCloudProviderService(Guid organizationId, Guid organizationCPSId, OrganizationCPSPutRp resource)
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

            OrganizationCPS organizationCPS = organization.GetCloudProviderServiceById(organizationCPSId);
            if (organizationCPS == null)
            {
                await _domainManagerService.AddConflict($"The cloud provider service with id {organizationCPSId} does not exists.");
                return;
            }

            bool validCredentials = await _cpsCredentialService(resource.Type).ValidateCredentials(resource.AccessId, resource.AccessName, resource.AccessSecret, resource.AccessAppId,
                                                                                                  resource.AccessAppSecret, resource.AccessDirectory, resource.AccessRegion);

            if (!validCredentials)
            {
                await _domainManagerService.AddConflict($"The credentials are not valid or there are some permissions problems");
                return;
            }

            user.UpdateCloudProviderService(organizationId, organizationCPSId,
                                            _dataProtectorService.Protect(resource.AccessId),
                                            _dataProtectorService.Protect(resource.AccessName),
                                            _dataProtectorService.Protect(resource.AccessSecret),
                                            _dataProtectorService.Protect(resource.AccessAppId),
                                            _dataProtectorService.Protect(resource.AccessAppSecret),
                                            _dataProtectorService.Protect(resource.AccessDirectory),
                                            _dataProtectorService.Protect(resource.AccessRegion));

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

        }

        public async Task DeleteCloudProviderService(Guid organizationId, Guid organizationCPSId)
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

            OrganizationCPS organizationCPS = organization.GetCloudProviderServiceById(organizationCPSId);
            if (organizationCPS == null)
            {
                await _domainManagerService.AddConflict($"The cloud provider service with id {organizationCPSId} does not exists.");
                return;
            }

            List<Project> relatedProjects = organization.GetProjectsByCPSId(organizationCPSId);
            if (relatedProjects.Any())
            {
                await _domainManagerService.AddConflict($"There are projects already configured with the cloud provider service {organizationCPS.Name}.");
                return;
            }

            user.DeleteCloudProviderService(organizationId, organizationCPSId);
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
    }
}
