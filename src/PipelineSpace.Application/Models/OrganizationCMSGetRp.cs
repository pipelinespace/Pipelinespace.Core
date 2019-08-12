using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSListRp
    {
        public IReadOnlyList<OrganizationCMSListItemRp> Items { get; set; }
    }

    public class OrganizationCMSListItemRp
    {
        public Guid OrganizationCMSId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationManagementService Type { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccessId { get; set; }
        public string AccessSecret { get; set; }
        public string AccessToken { get; set; }
    }

    public class OrganizationCMSGetRp
    {
        public Guid OrganizationCMSId { get; set; }
        public string Name { get; set; }
        public ConfigurationManagementService Type { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccessId { get; set; }
        public string AccessSecret { get; set; }
        public string AccessToken { get; set; }
    }
}
