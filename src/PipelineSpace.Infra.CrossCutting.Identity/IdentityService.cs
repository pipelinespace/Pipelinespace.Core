using Microsoft.AspNetCore.Http;
using PipelineSpace.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.InvariantCultureIgnoreCase) || c.Type.Equals("sub", StringComparison.InvariantCultureIgnoreCase)).Value;
        }

        public string GetUserName()
        {
            return this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "fullname").Value;
        }
        
        public string GetUserEmail()
        {
            return this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "email").Value;
        }

        public string GetOwnerId()
        {
            return this._httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "owneridentifier").Value;
        }
    }
}
