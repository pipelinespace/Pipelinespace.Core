using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSEndpointRepositoryModel
    {
        public List<CMSVSTSEndpointRepositoryItemModel> Repositories { get; set; }
    }

    public class CMSVSTSEndpointRepositoryItemModel
    {
        public string DefaultBranch { get; set; }
        public string FullName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public CMSVSTSEndpointRepositoryPropertyModel Properties { get; set; }

        public string SourceProviderName { get; set; }
        public string Url { get; set; }
    }

    public class CMSVSTSEndpointRepositoryPropertyModel
    {
        public string ApiUrl { get; set; }
        public string BranchesUrl { get; set; }
        public string CloneUrl { get; set; }
        public string ConnectedServiceId { get; set; }
        public string DefaultBranch { get; set; }
        public string FullName { get; set; }
        public bool IsFork { get; set; }
        public bool IsPrivate { get; set; }
        public string LastUpdated { get; set; }
        public string ManageUrl { get; set; }
        public string OwnerAvatarUrl { get; set; }
        public string OwnerIsAUser { get; set; }
        public string RefsUrl { get; set; }
        public string SafeOwnerId { get; set; }
        public string SafeRepository { get; set; }
    }
}
