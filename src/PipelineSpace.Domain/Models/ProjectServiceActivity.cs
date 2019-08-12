﻿using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceActivity : BaseEntity
    {
        public ProjectServiceActivity()
        {

        }

        public Guid ProjectServiceActivityId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid ProjectServiceId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Log { get; set; }

        [Required]
        public ActivityStatus ActivityStatus { get; set; }

        public static class Factory
        {
            public static ProjectServiceActivity Create(string code, string name, string createdBy)
            {
                var entity = new ProjectServiceActivity()
                {
                    Code = code,
                    Name = name,
                    ActivityStatus = ActivityStatus.Pending,
                    Log = "Waiting",
                    CreatedBy = createdBy
                };

                var validationResult = new DataValidatorManager<ProjectServiceActivity>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
