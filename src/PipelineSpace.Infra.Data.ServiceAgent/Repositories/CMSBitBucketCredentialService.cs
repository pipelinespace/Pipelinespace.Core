using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent.Repositories
{
    public class CMSBitBucketCredentialService : ICMSCredentialService
    {
        private readonly IExternalAuthTokenService _externalAuthTokenService;
        private const string API_URL = "https://api.bitbucket.org";
        public CMSBitBucketCredentialService(IExternalAuthTokenService externalAuthTokenService)
        {
            this._externalAuthTokenService = externalAuthTokenService;
        }

        public CMSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null, string accessToken = null)
        {
            return new CMSAuthCredentialModel {

                AccountId = accountId,
                AccessToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", accountId, accessToken))),
                Type = "Bearer",
                Url = API_URL,
                Provider = "BitBucket"
            };
        }
    }
}
