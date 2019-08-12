using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceEnvironment : BaseEntity
    {
        public Guid ProjectServiceEnvironmentId { get; set; }

        [Required]
        public Guid ProjectEnvironmentId { get; set; }
        
        public virtual ProjectEnvironment ProjectEnvironment { get; set; }

        public virtual List<ProjectServiceEnvironmentVariable> Variables { get; set; }

        public string LastStatus { get; set; }

        public string LastStatusCode { get; set; }
        
        public string LastVersionId { get; set; }

        public string LastVersionName { get; set; }
        
        public string LastSuccessVersionId { get; set; }

        public string LastSuccessVersionName { get; set; }

        public string LastApprovalId { get; set; }

        public DateTime LastEventDate { get; set; }

        public void SetVariable(string name, string value)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectServiceEnvironmentVariable>();

            var projectEnvironmentVariable = this.GetVariableByName(name);
            if (projectEnvironmentVariable == null)
            {
                throw new ApplicationException($"The variable {name} does not exists.");
            }

            projectEnvironmentVariable.Value = value;
        }

        public ProjectServiceEnvironmentVariable GetVariableByName(string name)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectServiceEnvironmentVariable>();

            return this.Variables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public void AddVariable(string name, string value)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectServiceEnvironmentVariable>();

            var projectEnvironmentVariable = ProjectServiceEnvironmentVariable.Factory.Create(this.ProjectServiceEnvironmentId, name, value, this.CreatedBy);

            this.Variables.Add(projectEnvironmentVariable);
        }

        public static class Factory
        {
            public static ProjectServiceEnvironment Create(Guid projectEnvironmentId, string createdby)
            {
                var entity = new ProjectServiceEnvironment()
                {
                    ProjectServiceEnvironmentId = Guid.NewGuid(),
                    ProjectEnvironmentId = projectEnvironmentId,
                    CreatedBy = createdby,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectServiceEnvironment>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
