using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Identity
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.InvariantCultureIgnoreCase) || c.Type.Equals("sub", StringComparison.InvariantCultureIgnoreCase))?.Value;
        }
    }
}
