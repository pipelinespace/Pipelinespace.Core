using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CMSVSTSReleaseModel
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
        public List<CMSVSTSReleaseArtifactModel> Artifacts { get; set; }

        public static CMSVSTSReleaseModel Create(int definitionId, string alias, int id, string name, string description)
        {
            var entity = new CMSVSTSReleaseModel()
            {
                DefinitionId = definitionId,
                Description = description,
                IsDraft = false,
                Reason = "none",
                ManualEnvironments = null,
                Artifacts = new List<CMSVSTSReleaseArtifactModel>()
                {
                    new CMSVSTSReleaseArtifactModel()
                    {
                        Alias = alias,
                        InstanceReference = new CMSVSTSReleaseArtifactInstanceModel()
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

    public class CMSVSTSReleaseArtifactModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("instanceReference")]
        public CMSVSTSReleaseArtifactInstanceModel InstanceReference { get; set; }
    }

    public class CMSVSTSReleaseArtifactInstanceModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
