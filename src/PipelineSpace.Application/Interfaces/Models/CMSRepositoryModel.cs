using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSRepositoryListModel : CMSBaseResultModel
    {
        public IReadOnlyList<CMSRepositoryListItemModel> Items { get; set; }
    }

    public class CMSRepositoryListItemModel
    {
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string SSHUrl { get; set; }
        public string DefaultBranch { get; set; }
    }
}
