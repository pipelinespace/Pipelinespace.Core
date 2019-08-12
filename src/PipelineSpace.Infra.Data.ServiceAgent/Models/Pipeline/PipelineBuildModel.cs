using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.Pipeline
{
    public class PipelineBuildModel
    {
        [JsonProperty("queue")]
        public PipelineBuildQueueModel Queue { get; set; }

        [JsonProperty("definition")]
        public PipelineBuildDefinitionModel Definition { get; set; }

        [JsonProperty("project")]
        public PipelineBuildProjectModel Project { get; set; }

        [JsonProperty("sourceBranch")]
        public string SourceBranch { get; set; }

        [JsonProperty("sourceVersion")]
        public string SourceVersion { get; set; }

        [JsonProperty("reason")]
        public int Reason { get; set; }

        public static PipelineBuildModel Create(int queueId, int definitionId, string projectId, string sourceBranch)
        {
            var entity = new PipelineBuildModel()
            {
                Queue = new PipelineBuildQueueModel()
                {
                    Id = queueId
                },
                Definition = new PipelineBuildDefinitionModel()
                {
                    Id = definitionId
                },
                Project = new PipelineBuildProjectModel()
                {
                    Id = projectId
                },
                SourceBranch = sourceBranch,
                Reason = 1
            };

            return entity;
        }
    }

    public class PipelineBuildQueueModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class PipelineBuildDefinitionModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class PipelineBuildProjectModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
