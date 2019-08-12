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
    public class DashboardQueryService : IDashboardQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public DashboardQueryService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
        }
        public async Task<DashboardGetRp> GetStats()
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            var organizations = user.FindOrganizations();
            var projects = organizations.SelectMany(c=> c.Projects);
            var pipes = projects.SelectMany(c => c.Services);
            var features = projects.SelectMany(c => c.Features);

            var model = new DashboardGetRp();

            model.CurrentOrganization = organizations.Count;
            model.CurrentProjects = projects.Count();
            model.CurrentPipes = pipes.Count();
            model.CurrentFeatures = features.Count();
            model.OrganizationItems = organizations.Select(c => new OrganizationListItemRp { OrganizationId = c.OrganizationId, Name = c.Name, Description = c.Description }).ToList();
            
            return model;
        }
    }
}
