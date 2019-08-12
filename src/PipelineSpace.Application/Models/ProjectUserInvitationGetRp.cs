using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectUserInvitationListRp
    {
        public ProjectUserInvitationListRp()
        {
            this.Items = new List<ProjectUserInvitationListItemRp>();
        }

        public IReadOnlyList<ProjectUserInvitationListItemRp> Items { get; set; }
    }

    public class ProjectUserInvitationListItemRp
    {
        public Guid InvitationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserInvitationStatus InvitationStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }

    public class ProjectUserInvitationGetRp
    {
        public Guid InvitationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string UserEmail { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineRole Role { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserInvitationStatus InvitationStatus { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }
}
