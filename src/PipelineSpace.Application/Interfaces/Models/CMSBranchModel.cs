using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSBranchListModel : CMSBaseResultModel
    {
        public IReadOnlyList<CMSBranchListItemModel> Items { get; set; }
    }

    public class CMSBranchListItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
