using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface IIdentityService
    {
        string GetUserId();
        string GetUserName();
        string GetUserEmail();
        string GetOwnerId();
    }
}
