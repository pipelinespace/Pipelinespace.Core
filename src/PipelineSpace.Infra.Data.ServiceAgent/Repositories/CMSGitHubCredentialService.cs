using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSGitHubCredentialService : ICMSCredentialService
    {
        private readonly IExternalAuthTokenService _externalAuthTokenService;
        private const string API_URL = "https://api.github.com";
        public CMSGitHubCredentialService(IExternalAuthTokenService externalAuthTokenService)
        {
            this._externalAuthTokenService = externalAuthTokenService;
        }

        public CMSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null, string accessToken = null)
        {
            return new CMSAuthCredentialModel {
                AccountId = accountId,
                AccessToken = string.IsNullOrEmpty(accessToken) ? this._externalAuthTokenService.GetAccessToken("GitHub") : accessToken,
                Type = "Bearer",
                Url = API_URL,
                Provider = "GitHub"
            };
        }
    }
}
