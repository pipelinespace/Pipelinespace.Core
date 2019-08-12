using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectEnvironment : BaseEntity
    {
        public Guid ProjectEnvironmentId { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public EnvironmentType Type { get; set; }
        
        public virtual Project Project { get; set; }

        public virtual List<ProjectEnvironmentVariable> Variables { get; set; }

        [Required]
        public int Rank { get; set; }
        
        [Required]
        public bool RequiresApproval { get; set; }

        [Required]
        public bool AutoProvision { get; set; }
        
        public void AddVariable(string name, string value)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectEnvironmentVariable>();

            var projectEnvironmentVariable = ProjectEnvironmentVariable.Factory.Create(this.ProjectEnvironmentId, name, value, this.CreatedBy);

            this.Variables.Add(projectEnvironmentVariable);
        }

        public void SetVariable(string name, string value)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectEnvironmentVariable>();

            var projectEnvironmentVariable = this.GetVariableByName(name);
            if(projectEnvironmentVariable == null)
            {
                throw new ApplicationException($"The variable {name} does not exists.");
            }

            projectEnvironmentVariable.Value = value;
        }

        public ProjectEnvironmentVariable GetVariableByName(string name)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectEnvironmentVariable>();

            return this.Variables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static class Factory
        {
            public static ProjectEnvironment Create(string name, string description, EnvironmentType type, bool requiresApproval, bool autoProvision, int rank, string createdBy)
            {
                var entity = new ProjectEnvironment()
                {
                    Name = name,
                    Description = description,
                    Type = type,
                    RequiresApproval = requiresApproval,
                    AutoProvision = autoProvision,
                    Rank = rank,
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<ProjectEnvironment>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<ProjectEnvironment>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);
        }
    }


}
