using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.VSTS
{
    public class CMSVSTSProjectCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("capabilities")] 
        public CMSVSTSProjectCapabilityModel Capabilities { get; set; }
    }

    public class CMSVSTSProjectCapabilityModel
    {
        [JsonProperty("versioncontrol")]
        public CMSVSTSProjectVersionControlModel VersionControl { get; set; }
        [JsonProperty("processTemplate")]
        public CMSVSTSProjectProcessTemplateModel ProcessTemplate { get; set; }
    }

    public class CMSVSTSProjectVersionControlModel
    {
        [JsonProperty("sourceControlType")]
        public string SourceControlType { get; set; }
    }

    public class CMSVSTSProjectProcessTemplateModel
    {
        [JsonProperty("templateTypeId")]
        public string TemplateTypeId { get; set; }
    }
}
