using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<CMSVSTSObjectRepositoryModel> GetTemplateBuildDefinition(string repository, string templateName, string definitionName, CMSAuthCredentialModel authCredential);
        Task<CMSVSTSObjectRepositoryModel> GetTemplateInfraDefinition(string repository, string templateName, string definitionName, CMSAuthCredentialModel authCredential);
    }
}
