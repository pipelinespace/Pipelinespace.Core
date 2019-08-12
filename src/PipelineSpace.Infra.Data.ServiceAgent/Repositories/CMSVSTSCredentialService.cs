using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSVSTSCredentialService : ICMSCredentialService
    {
        private readonly IExternalAuthTokenService _externalAuthTokenService;
        private const string API_URL = "https://app.vssps.visualstudio.com";
        public CMSVSTSCredentialService(IExternalAuthTokenService externalAuthTokenService)
        {
            this._externalAuthTokenService = externalAuthTokenService;
        }

        public CMSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null, string accessToken = null)
        {
            return new CMSAuthCredentialModel
            {
                AccountId = accountId,
                AccessToken = string.IsNullOrEmpty(accessSecret) ? this._externalAuthTokenService.GetAccessToken("VSTS") : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", accessSecret))),
                Type = string.IsNullOrEmpty(accessSecret) ? "Bearer" : "Basic",
                Url = string.IsNullOrEmpty(accountId) ? API_URL : $"https://{accountName}.visualstudio.com",
                Provider = "VSTS"
            };
        }
    }
}
