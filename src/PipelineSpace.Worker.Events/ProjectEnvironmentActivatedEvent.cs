using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectEnvironmentActivatedEvent : BaseEvent
    {
        public ProjectEnvironmentActivatedEvent(string correlationId) : base(correlationId)
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

        /// <summary>
        /// Organization Name
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Service Name
        /// </summary>
        public string ServiceName { get; set; }

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

        /// <summary>
        /// Release Stage Id From the Service
        /// </summary>
        public int ReleseStageId { get; set; }

        public string EnvironmentName { get; set; }
    }
}
