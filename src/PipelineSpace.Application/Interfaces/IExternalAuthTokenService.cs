using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface IExternalAuthTokenService
    {
        string GetAccessToken(string provider);
        string GetSchemeType(string provider);
    }
}
