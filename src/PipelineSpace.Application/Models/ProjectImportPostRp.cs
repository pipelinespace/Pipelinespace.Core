using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectImportPostRp
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
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
        
        [Required]
        public string AgentPoolId { get; set; }

        public string ProjectExternalId { get; set; }
        public string ProjectExternalName { get; set; }

        //[Required]
        public string BuildDefinitionYML { get; set; }
        //[Required]
        public Guid? ProjectServiceTemplateId { get; set; }

        [Required]
        public ProjectVisibility projectVisibility { get; set; }

        public List<ProjectRepositoryImportPostRp> Repositories { get; set; }
    }

    public class ProjectRepositoryImportPostRp
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public object ExternalName { get; set; }
    }

}
