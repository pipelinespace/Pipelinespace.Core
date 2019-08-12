using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class OrganizationProjectServiceTemplatePostRp
    {
        [Required]
        public Guid SourceProjectServiceTemplateId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        public PipeType PipeType { get; set; }

        [Required]
        public ConfigurationManagementService RepositoryCMSType { get; set; }

        [Required]
        public Guid ProgrammingLanguageId { get; set; }

        [Required]
        public string Framework { get; set; }

        [Required]
        public Guid ConnectionId { get; set; }

        public string TeamId { get; set; }
        public string ProjectExternalId { get; set; }

        public string ProjectExternalName { get; set; }
    }
}
