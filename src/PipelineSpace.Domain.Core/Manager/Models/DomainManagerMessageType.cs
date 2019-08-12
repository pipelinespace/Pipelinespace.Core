using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.Core.Manager.Models
{
    public enum DomainManagerMessageType
    {
        NotFound,
        Conflict,
        Result,
        BadRequest,
        Unauthorized
    }
}
