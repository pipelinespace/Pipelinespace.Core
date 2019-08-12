using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeature : BaseEntity
    {
        public Guid ProjectFeatureId { get; set; }
        
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual List<ProjectFeatureEnvironment> Environments { get; set; }

        public virtual List<ProjectFeatureService> Services { get; set; }

        public DateTime? CompletionDate { get; set; }

        public ProjectFeatureService GetFeatureServiceById(Guid projectServiceId)
        {
            if (this.Services == null)
                this.Services = new List<ProjectFeatureService>();

            return this.Services.FirstOrDefault(x => x.ProjectServiceId == projectServiceId);
        }

        public void AddService(Guid serviceId, List<ProjectServiceEnvironmentVariable> variables)
        {
            if (this.Services == null)
                this.Services = new List<ProjectFeatureService>();

            var projectFeatureService = ProjectFeatureService.Factory.Create(this.ProjectId, this.ProjectFeatureId, serviceId, this.CreatedBy);

            projectFeatureService.AddEnvironmentsAndVariables(this.Environments, variables);

            this.Services.Add(projectFeatureService);

            //set status to preparing. it will be active when build/release are finished.
            this.Status = EntityStatus.Preparing;
        }

        public void DeleteService(Guid serviceId, string userId)
        {
            if (this.Services == null)
                this.Services = new List<ProjectFeatureService>();

            var projectFeatureService = this.Services.FirstOrDefault(x => x.ProjectServiceId == serviceId);

            if(projectFeatureService != null)
            {
                projectFeatureService.Delete(userId);
            }
        }

        public List<ProjectFeatureService> GetPreparingServices()
        {
            if (this.Services == null)
                this.Services = new List<ProjectFeatureService>();

            return this.Services.Where(x => x.Status == EntityStatus.Preparing).ToList();
        }

        public List<ProjectFeatureService> GetServices()
        {
            if (this.Services == null)
                this.Services = new List<ProjectFeatureService>();

            return this.Services.Where(x => x.Status == EntityStatus.Active).ToList();
        }

        public void Complete()
        {
            this.CompletionDate = DateTime.UtcNow;
        }

        public void AddEnvironment(ProjectFeatureEnvironment environment)
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureEnvironment>();

            this.Environments.Add(environment);
        }

        public List<ProjectFeatureEnvironment> GetEnvironments()
        {
            if (this.Environments == null)
                this.Environments = new List<ProjectFeatureEnvironment>();

            return this.Environments.Where(x => x.Status == EntityStatus.Active).ToList();
        }

        public static class Factory
        {
            public static ProjectFeature Create(string name, string description, string createdBy)
            {
                var entity = new ProjectFeature()
                {
                    ProjectFeatureId = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<ProjectFeature>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }

        public string GetStatusName()
        {
            //if (this.CompletionDate.HasValue)
            //{
            //    return "Completed";
            //}

            return Status.ToString();
        }

        public bool IsCompleted()
        {
            return this.CompletionDate.HasValue;
        }

        public void ValidateBasicConstraints()
        {
            var validationResult = new DataValidatorManager<ProjectFeature>().Build().Validate(this);
            if (!validationResult.IsValid)
                throw new ApplicationException(validationResult.Errors);
        }
    }
}
