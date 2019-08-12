using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationProjectServiceTemplateListRp
    {
        public List<OrganizationProjectServiceTemplateListItemRp> Items { get; set; }
    }

    public class OrganizationProjectServiceTemplateListItemRp
    {
        public Guid ProjectServiceTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateAccess Access { get; set; }
        public string ProgrammingLanguageName { get; set; }
        public string Framework { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
        public string Logo { get; internal set; }
    }

    public class OrganizationProjectServiceTemplateGetRp
    {
        public Guid ProjectServiceTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateAccess Access { get; set; }
        public string ProgrammingLanguageName { get; set; }
        public string Framework { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityStatus Status { get; set; }
    }
}
