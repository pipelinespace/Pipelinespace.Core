using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSBranchModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("newObjectId")]
        public string NewObjectId { get; set; }

        [JsonProperty("oldObjectId")]
        public string OldObjectId { get; set; }
    }

    public class CMSVSTSBranchListModel
    {
        [JsonProperty("value")]
        public List<CMSVSTSBranchListItemModel> Items { get; set; }
    }

    public class CMSVSTSBranchListItemModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }
    }
}
