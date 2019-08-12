using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ICMSService
    {
        Task<CMSProjectAvailabilityResultModel> ValidateProjectAvailability(CMSAuthCredentialModel authCredential, string organization, string name);
        Task<CMSProjectCreateResultModel> CreateProject(CMSAuthCredentialModel authCredential, CMSProjectCreateModel model);
        Task<CMSServiceAvailabilityResultModel> ValidateServiceAvailability(CMSAuthCredentialModel authCredential, string teamId, string projectExternalId, string projectName, string name);
        Task<CMSServiceCreateResultModel> CreateService(CMSAuthCredentialModel authCredential, CMSServiceCreateModel model);
    }
}
