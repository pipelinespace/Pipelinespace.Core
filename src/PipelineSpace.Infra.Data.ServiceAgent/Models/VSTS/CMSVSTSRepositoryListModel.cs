using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSRepositoryListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSVSTSRepositoryListItemModel> Items { get; set; }
    }

    public class CMSVSTSRepositoryListItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }
        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; }
        [JsonProperty("sshUrl")]
        public string SSHUrl { get; set; }
        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }

    }
}
