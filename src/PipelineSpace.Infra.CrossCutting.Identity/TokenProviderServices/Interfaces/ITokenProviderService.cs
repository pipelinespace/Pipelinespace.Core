using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Interfaces
{
    public interface ITokenProviderService
    {
        Task<OAuthToken> RefreshToken(string refreshToken);
    }
}
