using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSProjectListModel : CMSBaseResultModel
    {
        [JsonProperty("value")]
        public IReadOnlyList<CMSProjectListItemModel> Items { get; set; }
    }

    public class CMSProjectListItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class CMSProjectModel : CMSBaseResultModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("_links")]
        public CMSProjectLinkModel Link { get; set; }
    }

    public class CMSProjectLinkModel
    {
        [JsonProperty("self")]
        public CMSProjectLinkRefModel Self { get; set; }
        [JsonProperty("web")]
        public CMSProjectLinkRefModel Web { get; set; }
    }

    public class CMSProjectLinkRefModel
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
