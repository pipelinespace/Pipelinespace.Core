using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.Pipeline
{
    public class PipelineReleaseModel
    {
        [JsonProperty("definitionId")]
        public int DefinitionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isDraft")]
        public bool IsDraft { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("manualEnvironments")]
        public object ManualEnvironments { get; set; }

        [JsonProperty("artifacts")]
        public List<PipelineReleaseArtifactModel> Artifacts { get; set; }

        public static PipelineReleaseModel Create(int definitionId, string alias, int id, string name, string description)
        {
            var entity = new PipelineReleaseModel()
            {
                DefinitionId = definitionId,
                Description = description,
                IsDraft = false,
                Reason = "none",
                ManualEnvironments = null,
                Artifacts = new List<PipelineReleaseArtifactModel>()
                {
                    new PipelineReleaseArtifactModel()
                    {
                        Alias = alias,
                        InstanceReference = new PipelineReleaseArtifactInstanceModel()
                        {
                            Id = id,
                            Name = name
                        }
                    }
                }
            };

            return entity;
        }
    }

    public class PipelineReleaseArtifactModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("instanceReference")]
        public PipelineReleaseArtifactInstanceModel InstanceReference { get; set; }
    }

    public class PipelineReleaseArtifactInstanceModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
