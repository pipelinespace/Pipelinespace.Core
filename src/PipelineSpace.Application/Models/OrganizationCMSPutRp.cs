using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationCMSPutRp
    {
        [Required]
        public string AccessId { get; set; }
        
        public string AccessSecret { get; set; }
    }
}
