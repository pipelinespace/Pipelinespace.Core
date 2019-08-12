using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCPSListRp
    {
        public IReadOnlyList<OrganizationCPSListItemRp> Items { get; set; }
    }

    public class OrganizationCPSListItemRp
    {
        public Guid OrganizationCPSId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CloudProviderService Type { get; set; }
        public string AccessId { get; set; }
        public string AccessName { get; set; }
        public string AccessSecret { get; set; }
        public string AccessRegion { get; set; }
        public string AccessAppId { get; set; }
        public string AccessAppSecret { get; set; }
        public string AccessDirectory { get; set; }
    }

    public class OrganizationCPSGetRp
    {
        public Guid OrganizationCPSId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CloudProviderService Type { get; set; }
        public string AccessId { get; set; }
        public string AccessName { get; set; }
        public string AccessSecret { get; set; }
        public string AccessRegion { get; set; }
        public string AccessAppId { get; set; }
        public string AccessAppSecret { get; set; }
        public string AccessDirectory { get; set; }
    }
}
