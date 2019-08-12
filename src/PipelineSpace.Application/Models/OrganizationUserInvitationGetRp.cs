using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationUserInvitationListRp
    {
        public OrganizationUserInvitationListRp()
        {
            this.Items = new List<OrganizationUserInvitationListItemRp>();
        }

        public IReadOnlyList<OrganizationUserInvitationListItemRp> Items { get; set; }
    }

    public class OrganizationUserInvitationListItemRp
    {
        public Guid InvitationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserInvitationStatus InvitationStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }

    public class OrganizationUserInvitationGetRp
    {
        public Guid InvitationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserInvitationStatus InvitationStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }
}
