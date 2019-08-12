using Microsoft.Extensions.Configuration;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Interfaces;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PipelineSpace.Infra.CrossCutting.Identity
{
    public class GitHubTokenProviderService : ITokenProviderService
    {
        readonly IConfiguration _configuration;
        public GitHubTokenProviderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<OAuthToken> RefreshToken(string refreshToken)
        {
            return await Task.FromResult<OAuthToken>(null);
        }
    }
}
