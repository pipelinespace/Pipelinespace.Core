using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices.Interfaces
{
    public interface IInternalProjectServiceTemplateService
    {
        Task ActivateProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId);
    }
}
