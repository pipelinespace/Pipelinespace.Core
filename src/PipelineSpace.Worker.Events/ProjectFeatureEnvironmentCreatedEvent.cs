using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectFeatureEnvironmentCreatedEvent : BaseEvent
    {
        public ProjectFeatureEnvironmentCreatedEvent(string correlationId) : base(correlationId)
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

        /// <summary>
        /// Release Stage Id From the Feature
        /// </summary>
        public int ReleseStageId { get; set; }

        public List<ProjectFeatureEnvironmentItemCreatedEvent> Environments { get; set; }
    }

    public class ProjectFeatureEnvironmentItemCreatedEvent
    {
        public string Name { get; set; }
        public bool RequiredApproval { get; set; }
        public List<ProjectFeatureEnvironmentItemVariableCreatedEvent> Variables { get; set; }
        public int Rank { get; set; }
    }

    public class ProjectFeatureEnvironmentItemVariableCreatedEvent
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
