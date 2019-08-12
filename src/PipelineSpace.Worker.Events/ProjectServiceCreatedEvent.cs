using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectServiceCreatedEvent : BaseEvent
    {
        public ProjectServiceCreatedEvent(string correlationId) : base(correlationId)
        {
        }

        /// <summary>
        /// Current User
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Organization Id
        /// </summary>
        public Guid OrganizationId { get; set; }
        
        /// <summary>
        /// Project Id
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Service Id
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// External Project Id from CMS
        /// </summary>
        public string ProjectExternalId { get; set; }

        /// <summary>
        /// External Project Endpoint Id from CMS - Cloud
        /// </summary>
        public string ProjectExternalEndpointId { get; set; }

        /// <summary>
        /// External Project Endpoint Id from CMS - Git
        /// </summary>
        public string ProjectExternalGitEndpoint { get; set; }

        /// <summary>
        /// VSTS Project Fake Name
        /// </summary>
        public string ProjectVSTSFakeName { get; set; }

        /// <summary>
        /// VSTS Project Fake Id
        /// </summary>
        public string ProjectVSTSFakeId { get; set; }

        /// <summary>
        /// External Service Id from CMS (Repository)
        /// </summary>
        public string ServiceExternalId { get; set; }

        /// <summary>
        /// External Service Url from CMS (Repository)
        /// </summary>
        public string ServiceExternalUrl { get; set; }

        /// <summary>
        /// Project Name (project name to replace in new repository namespace project)
        /// </summary>
        public string ProjectName { get; set; }
        public string InternalProjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Service Name (Repository name to create in CMS, service name to replace in new repository namespace project)
        /// </summary>
        public string ServiceName { get; set; }
        public string InternalServiceName { get; set; }
        /// <summary>
        /// Repository Template (Repository template to create in CMS)
        /// </summary>
        public string ServiceTemplateUrl { get; set; }

        public string AgentPoolId { get; set; }
        
        public ConfigurationManagementService CMSType { get; set; }
        public string CMSAccountId { get; set; }
        public string CMSAccountName { get; set; }
        public string CMSAccessId { get; set; }
        public string CMSAccessSecret { get; set; }
        public string CMSAccessToken { get; set; }

        public CloudProviderService CPSType { get; set; }
        public string CPSAccessId { get; set; }
        public string CPSAccessName { get; set; }
        public string CPSAccessSecret { get; set; }
        public string CPSAccessRegion { get; set; }

        public List<ProjectServiceTemplateParameterCreatedEvent> TemplateParameters { get; set; }

        
        public TemplateAccess TemplateAccess { get; set; }
        public bool NeedCredentials { get; set; }
        public ConfigurationManagementService RepositoryCMSType { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
        
    }

    public class ProjectServiceTemplateParameterCreatedEvent
    {
        public string VariableName { get; set; }
        public string Value { get; set; }
        public string Scope { get; set; }
    }
}
