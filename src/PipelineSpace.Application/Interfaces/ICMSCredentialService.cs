using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICMSCredentialService
    {
        CMSAuthCredentialModel GetToken(string accountId = null, string accountName = null, string accessSecret = null, string accessToken = null);
    }
}
