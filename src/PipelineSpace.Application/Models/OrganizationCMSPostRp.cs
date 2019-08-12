using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSPostRp
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public ConfigurationManagementService Type { get; set; }

        [Required]
        public CMSConnectionType ConnectionType { get; set; }

        [Required]
        public string AccessId { get; set; }
        
        public string AccessSecret { get; set; }

        [Required]
        public string AccessToken { get; set; }
    }
}
