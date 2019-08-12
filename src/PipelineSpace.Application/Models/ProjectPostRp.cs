using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectPostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        public ProjectType ProjectType { get; set; }

        [Required(ErrorMessage = "The Git Provider Connection is required")]
        public Guid OrganizationCMSId { get; set; }

        //[Required(ErrorMessage = "The Cloud Provider Connection is required")]
        public Guid? OrganizationCPSId { get; set; }

        public Guid? ProjectTemplateId { get; set; }

        public string TeamId { get; set; }

        [Required]
        public string AgentPoolId { get; set; }

        [Required]
        public ProjectVisibility projectVisibility { get; set; }
    }

}
