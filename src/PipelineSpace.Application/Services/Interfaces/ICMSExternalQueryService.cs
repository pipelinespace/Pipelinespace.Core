﻿using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.Interfaces
{
    public interface ICMSExternalQueryService
    {
        Task<CMSProjectListRp> GetAccounts(ConfigurationManagementService type);

    }
}
