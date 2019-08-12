using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationUserInvitationPostRp
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public PipelineRole Role { get; set; }
    }
}
