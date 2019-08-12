using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain;
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
    public class OrganizationProjectServiceTemplateQueryService : IOrganizationProjectServiceTemplateQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly IProjectServiceTemplateRepository _projectServiceTemplateRepository;

        public OrganizationProjectServiceTemplateQueryService(IDomainManagerService domainManagerService,
                                                              IIdentityService identityService, 
                                                              IUserRepository userRepository,
                                                              IProjectServiceTemplateRepository projectServiceTemplateRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _projectServiceTemplateRepository = projectServiceTemplateRepository;
        }


        public async Task<OrganizationProjectServiceTemplateListRp> GetAllOrganizationProjectServiceTemplates(Guid organizationId)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            var organizationTemplates = organization.GetAllProjectServiceTemplates();
            OrganizationProjectServiceTemplateListRp list = new OrganizationProjectServiceTemplateListRp
            {
                Items = organizationTemplates.Select(x => new OrganizationProjectServiceTemplateListItemRp()
                {
                    ProjectServiceTemplateId = x.ProjectServiceTemplateId,
                    Name = x.Name,
                    Description = x.Description,
                    Access = x.TemplateAccess,
                    ProgrammingLanguageName = x.ProgrammingLanguage.Name,
                    Framework = x.Framework,
                    Status = x.Status,
                    Logo = x.Logo
                }).ToList()
            };

            return list;
        }

        public async Task<OrganizationProjectServiceTemplateListRp> GetOrganizationProjectServiceTemplates(Guid organizationId, CloudProviderService cloudProviderType, PipeType pipeType)
        {
            var loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return null;
            }

            var systemTemplates = await _projectServiceTemplateRepository.GetProjectServiceTemplates(ConfigurationManagementService.VSTS, cloudProviderType, pipeType);
            OrganizationProjectServiceTemplateListRp list = new OrganizationProjectServiceTemplateListRp
            {
                Items = systemTemplates.Select(x => new OrganizationProjectServiceTemplateListItemRp()
                {
                    ProjectServiceTemplateId = x.ProjectServiceTemplateId,
                    Name = x.Name,
                    Description = x.Description,
                    Access = x.TemplateAccess,
                    ProgrammingLanguageName = x.ProgrammingLanguage.Name,
                    Framework = x.Framework,
                    Status = x.Status,
                    Logo = x.Logo
                }).ToList()
            };

            if (cloudProviderType == CloudProviderService.None)
            {
                var organizationAWSTemplates = organization.GetProjectServiceTemplates(CloudProviderService.AWS, pipeType);
                var organizationAzureTemplates = organization.GetProjectServiceTemplates(CloudProviderService.Azure, pipeType);

                foreach (var item in organizationAWSTemplates)
                {
                    list.Items.Add(new OrganizationProjectServiceTemplateListItemRp()
                    {
                        ProjectServiceTemplateId = item.ProjectServiceTemplateId,
                        Name = item.Name,
                        Description = item.Description,
                        Access = item.TemplateAccess,
                        ProgrammingLanguageName = item.ProgrammingLanguage.Name,
                        Framework = item.Framework,
                        Status = item.Status
                    });
                }

                foreach (var item in organizationAzureTemplates)
                {
                    list.Items.Add(new OrganizationProjectServiceTemplateListItemRp()
                    {
                        ProjectServiceTemplateId = item.ProjectServiceTemplateId,
                        Name = item.Name,
                        Description = item.Description,
                        Access = item.TemplateAccess,
                        ProgrammingLanguageName = item.ProgrammingLanguage.Name,
                        Framework = item.Framework,
                        Status = item.Status
                    });
                }
            }
            else {
                var organizationTemplates = organization.GetProjectServiceTemplates(cloudProviderType, pipeType);
                foreach (var item in organizationTemplates)
                {
                    list.Items.Add(new OrganizationProjectServiceTemplateListItemRp()
                    {
                        ProjectServiceTemplateId = item.ProjectServiceTemplateId,
                        Name = item.Name,
                        Description = item.Description,
                        Access = item.TemplateAccess,
                        ProgrammingLanguageName = item.ProgrammingLanguage.Name,
                        Framework = item.Framework,
                        Status = item.Status
                    });
                }
            }
           
            
            return list;
        }

    }
}
