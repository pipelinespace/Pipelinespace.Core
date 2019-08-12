﻿using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeatureServiceEnvironment : BaseEntity
    {
        public Guid ProjectFeatureServiceEnvironmentId { get; set; }

        [Required]
        public Guid ProjectFeatureEnvironmentId { get; set; }
        
        public virtual ProjectFeatureEnvironment ProjectFeatureEnvironment { get; set; }

        public virtual List<ProjectFeatureServiceEnvironmentVariable> Variables { get; set; }

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
                this.Variables = new List<ProjectFeatureServiceEnvironmentVariable>();

            var projectEnvironmentVariable = this.GetVariableByName(name);
            if (projectEnvironmentVariable == null)
            {
                throw new ApplicationException($"The variable {name} does not exists.");
            }

            projectEnvironmentVariable.Value = value;
        }

        public ProjectFeatureServiceEnvironmentVariable GetVariableByName(string name)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectFeatureServiceEnvironmentVariable>();

            return Variables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public void AddVariable(string name, string value)
        {
            if (this.Variables == null)
                this.Variables = new List<ProjectFeatureServiceEnvironmentVariable>();

            var projectEnvironmentVariable = ProjectFeatureServiceEnvironmentVariable.Factory.Create(this.ProjectFeatureServiceEnvironmentId, name, value, this.CreatedBy);

            this.Variables.Add(projectEnvironmentVariable);
        }

        public static class Factory
        {
            public static ProjectFeatureServiceEnvironment Create(Guid projectFeatureEnvironmentId, string createdby)
            {
                var entity = new ProjectFeatureServiceEnvironment()
                {
                    ProjectFeatureServiceEnvironmentId = Guid.NewGuid(),
                    ProjectFeatureEnvironmentId = projectFeatureEnvironmentId,
                    CreatedBy = createdby,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectFeatureServiceEnvironment>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
