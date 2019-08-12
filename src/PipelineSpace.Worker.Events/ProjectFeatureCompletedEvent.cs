using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectFeatureCompletedEvent : BaseEvent
    {
        public ProjectFeatureCompletedEvent(string correlationId) : base(correlationId)
        {
        }

        /// <summary>
        /// Organization Id
        /// </summary>
        public Guid OrganizationId { get; set; }
        
        /// <summary>
        /// Project Id
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Feature Id
        /// </summary>
        public Guid FeatureId { get; set; }

        /// <summary>
        /// Services
        /// </summary>
        public List<ProjectFeatureServiceCompletedEvent> Services { get; set; }
        
        /// <summary>
        /// External Project Id from CMS
        /// </summary>
        public string ProjectExternalId { get; set; }

        /// <summary>
        /// External Project Endpoint Id from CMS
        /// </summary>
        public string ProjectExternalEndpointId { get; set; }

        /// <summary>
        /// VSTS Project Fake 
        /// </summary>
        public string ProjectVSTSFakeName { get; set; }

        /// <summary>
        /// Project Name (project name to replace in new repository namespace project)
        /// </summary>
        public string ProjectName { get; set; }

        public string FeatureName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }

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
        public string CPSAccessAppId { get; set; }
        public string CPSAccessAppSecret { get; set; }
        public string CPSAccessDirectory { get; set; }

        public bool DeleteInfrastructure { get; set; }
        public string OrganizationExternalId { get; set; }
    }

    public class ProjectFeatureServiceCompletedEvent : BaseEvent
    {
        public ProjectFeatureServiceCompletedEvent(string correlationId) : base(correlationId)
        {

        }
        /// <summary>
        /// Service Id
        /// </summary>
        public Guid ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string ServiceTemplateUrl { get; set; }

        /// <summary>
        /// External Service Id from CMS (Repository)
        /// </summary>
        public string ServiceExternalId { get; set; }

        /// <summary>
        /// External Service Url from CMS (Repository)
        /// </summary>
        public string ServiceExternalUrl { get; set; }

        public int? ReleaseStageId { get; set; }

        public int? CommitStageId { get; set; }
    }
}
