using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServicePostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_.]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string RepositoryName { get; set; }

        [Required(ErrorMessage = "The agent pool is required")]
        public string AgentPoolId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The pipe template is required")]
        public Guid ProjectServiceTemplateId { get; set; }
    }

    public class ProjectServiceImportPostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The agent pool is required")]
        public string AgentPoolId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        public string ServiceExternalId { get; set; }

        [Required]
        public string ServiceExternalUrl { get; set; }
        [Required]
        public string ServiceExternalName { get; set; }

        [Required(ErrorMessage = "The pipe template is required")]
        public Guid ProjectServiceTemplateId { get; set; }

        [Required]
        public string BuildDefinitionYML { get; set; }
        [Required(ErrorMessage = "The branch is required")]
        public string BranchName { get; set; }
        [Required(ErrorMessage = "The Git Providers is required")]
        public Guid OrganizationCMSId { get; set; }
        public string ProjectExternalName { get; set; }
        public string ProjectExternalId { get; set; }
    }

    public class InternalProjectServicePostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The pipe template is required")]
        public Guid ProjectServiceTemplateId { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class InternalProjectServiceImportPostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }
        
        [Required]
        public string ServiceExternalId { get; set; }

        [Required]
        public string ServiceExternalUrl { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string BranchName { get; set; }
        [Required]
        public Guid OrganizationCMSId { get; set; }
        public string ServiceExternalName { get; set; }
        public string BuildDefinitionYML { get; set; }
        public string ProjectExternalId { get; set; }
        public string ProjectExternalName { get; set; }
        public Guid ProjectServiceTemplateId { get; set; }
    }
}
