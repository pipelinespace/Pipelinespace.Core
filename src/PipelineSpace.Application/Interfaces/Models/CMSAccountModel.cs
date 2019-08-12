using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSAccountListModel: CMSBaseResultModel
    {
        public List<CMSAccountListItemModel> Items { get; set; }
    }

    public class CMSAccountListItemModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOrganization { get; set; }
    }
}
