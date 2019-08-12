using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Options
{
    public class AuthenticationOptions
    {
        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string NameClaimType { get; set; }
        public string RoleClaimType { get; set; }
    }
}
