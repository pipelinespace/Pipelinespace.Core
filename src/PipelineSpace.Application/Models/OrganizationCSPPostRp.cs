using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCPSPostRp
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public CloudProviderService Type { get; set; }

        [Required]
        public string AccessId { get; set; }

        public string AccessName { get; set; }

        public string AccessSecret { get; set; }

        public string AccessAppId { get; set; }

        public string AccessAppSecret { get; set; }

        public string AccessDirectory { get; set; }

        [Required]
        public string AccessRegion { get; set; }
    }
}
