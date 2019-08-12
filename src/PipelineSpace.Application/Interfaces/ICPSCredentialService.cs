using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICPSCredentialService
    {
        CPSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null);
        Task<bool> ValidateCredentials(string accessId, string accessName, string accessSecret, string accessAppId, string accessAppSecret, string accessDirectory, string accessRegion);
    }
}
