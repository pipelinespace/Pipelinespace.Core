using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface IProjectCloudCredentialService
    {
        ProjectCloudCredentialModel ProjectCredentialResolver(OrganizationCMS organizationCMS, Project project);
        ProjectCloudCredentialModel ProjectServiceCredentialResolver(Project project, ProjectService projectService);
        ProjectCloudCredentialModel ProjectFeatureServiceCredentialResolver(Project project, ProjectFeatureService projectFeatureService);

    }
}
