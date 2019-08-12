using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
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
    public class CMSExternalQueryService : ICMSExternalQueryService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly Func<ConfigurationManagementService, ICMSQueryService> _cmsQueryService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;

        public CMSExternalQueryService(IDomainManagerService domainManagerService,
                                       Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                       Func<ConfigurationManagementService, ICMSQueryService> cmsQueryService,
                                       IIdentityService identityService,
                                       IUserRepository userRepository)
        {
            _domainManagerService = domainManagerService;
            _cmsQueryService = cmsQueryService;
            _cmsCredentialService = cmsCredentialService;
            _identityService = identityService;
            _userRepository = userRepository;
        }
        
        public async Task<CMSProjectListRp> GetAccounts(ConfigurationManagementService type)
        {
            
            var cmsAuthCredential = this._cmsCredentialService(type).GetToken();
            var cmsAccounts = await _cmsQueryService(type).GetAccounts(cmsAuthCredential);

            CMSProjectListRp list = new CMSProjectListRp();

            if (cmsAccounts != null && cmsAccounts.Items != null)
            {
                list.Items = cmsAccounts.Items.Select(c => new CMSProjectListItemRp { AccountId = c.AccountId, Name = c.Name, Description = c.Description }).ToList();
            }


            return list;
        }
        
    }
}
