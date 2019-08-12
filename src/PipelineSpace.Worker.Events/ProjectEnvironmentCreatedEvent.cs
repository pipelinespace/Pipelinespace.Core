using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectEnvironmentCreatedEvent : BaseEvent
    {
        public ProjectEnvironmentCreatedEvent(string correlationId) : base(correlationId)
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
        /// Release Stage Id From the Service
        /// </summary>
        public int ReleseStageId { get; set; }

        /// <summary>
        /// Name From the Service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// LastBuildSuccessVersionId  From the Service
        /// </summary>
        public string ServiceLastBuildSuccessVersionId { get; set; }

        /// <summary>
        /// LastBuildSuccessVersionName From the Service
        /// </summary>
        public string ServiceLastBuildSuccessVersionName { get; set; }

        public Guid EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public int EnvironmentRank { get; set; }
        public bool EnvironmentAutoProvision { get; set; }

        public List<ProjectEnvironmentItemCreatedEvent> Environments { get; set; }
    }

    public class ProjectEnvironmentItemCreatedEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool RequiredApproval { get; set; }
        public List<ProjectEnvironmentItemVariableCreatedEvent> Variables { get; set; }
        public int Rank { get; set; }
        public string LastSuccessVersionId { get; set; }
        public string LastSuccessVersionName { get; set; }
    }

    public class ProjectEnvironmentItemVariableCreatedEvent
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
