using Newtonsoft.Json;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSPipelineAgentQueueParamModel
    {
        public ConfigurationManagementService CMSType { get; set; }

        public string VSTSAPIVersion { get; set; }
        public string VSTSAccountName { get; set; }
        public string VSTSAccessSecret { get; set; }
        public string VSTSAccountProjectId { get; set; }

        public string ProjectName { get; set; }

        public string AgentPoolId { get; set; }
    }

    public class CMSPipelineAgentQueueResultModel
    {
        public int QueueId { get; set; }
        public string QueueName { get; set; }
        public int PoolId { get; set; }
        public string PoolName { get; set; }
    }

    public class CMSPipelineQueueListModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<CMSPipelineQueueListItemModel> Items { get; set; }
    }

    public class CMSPipelineQueueListItemModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pool")]
        public CMSPipelinePoolModel Pool { get; set; }
    }

    public class CMSPipelinePoolModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
