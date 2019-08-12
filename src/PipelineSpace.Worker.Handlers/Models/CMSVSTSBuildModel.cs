using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSBuildModel
    {
        [JsonProperty("queue")]
        public CMSVSTSBuildQueueModel Queue { get; set; }

        [JsonProperty("definition")]
        public CMSVSTSBuildDefinitionModel Definition { get; set; }

        [JsonProperty("project")]
        public CMSVSTSBuildProjectModel Project { get; set; }

        [JsonProperty("sourceBranch")]
        public string SourceBranch { get; set; }

        [JsonProperty("sourceVersion")]
        public string SourceVersion { get; set; }

        [JsonProperty("reason")]
        public int Reason { get; set; }

        public static CMSVSTSBuildModel Create(int queueId, int definitionId, string projectId, string sourceBranch)
        {
            var entity = new CMSVSTSBuildModel()
            {
                Queue = new CMSVSTSBuildQueueModel()
                {
                    Id = queueId
                },
                Definition = new CMSVSTSBuildDefinitionModel()
                {
                    Id = definitionId
                },
                Project = new CMSVSTSBuildProjectModel()
                {
                    Id = projectId
                },
                SourceBranch = sourceBranch,
                Reason = 1
            };

            return entity;
        }
    }

    public class CMSVSTSBuildQueueModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class CMSVSTSBuildDefinitionModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class CMSVSTSBuildProjectModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
