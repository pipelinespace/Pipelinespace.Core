
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Identity
{
    public class AspnetIdentityExternalTokenService : IExternalAuthTokenService
    {
        readonly IIdentityService _identityService;
        readonly Func<string, ITokenProviderService> _tokenProviderService;

        public AspnetIdentityExternalTokenService(IIdentityService identityService,
                                                  Func<string, ITokenProviderService> tokenProviderService)
        {
            this._identityService = identityService;
            this._tokenProviderService = tokenProviderService;
        }

        public string GetAccessToken(string provider)
        {
            return "";
        }

        public string GetSchemeType(string provider)
        {
            var userId = this._identityService.GetUserId();
            return "bearer";
        }
    }
}
