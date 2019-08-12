using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PipelineSpace.Worker.Events
{
    public class ProjectFeatureDeletedEvent : BaseEvent
    {
        public ProjectFeatureDeletedEvent(string correlationId) : base(correlationId)
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
        public List<ProjectFeatureServiceDeletedEvent> Services { get; set; }
        
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

        public string ProjectName { get; set; }

        public string FeatureName { get; set; }

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

        public SourceEvent SourceEvent { get; set; }
    }

    public class ProjectFeatureServiceDeletedEvent : BaseEvent
    {
        public ProjectFeatureServiceDeletedEvent(string correlationId) : base(correlationId)
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
        public Guid? CommitServiceHookId { get; set; }
        public Guid? ReleaseServiceHookId { get; set; }
        public Guid? CodeServiceHookId { get; set; }
        public Guid? ReleaseStartedServiceHookId { get; set; }
        public Guid? ReleasePendingApprovalServiceHookId { get; set; }
        public Guid? ReleaseCompletedApprovalServiceHookId { get; set; }

        /*Feature*************************************************************/
        public Guid OrganizationId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid FeatureId { get; set; }

        public string ProjectExternalId { get; set; }
        public string ProjectExternalEndpointId { get; set; }

        public string ProjectVSTSFakeName { get; set; }

        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
        public string FeatureName { get; set; }

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
    }
}
