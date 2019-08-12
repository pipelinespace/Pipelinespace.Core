using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectActivityPutRp
    {
        [Required]
        public string Log { get; set; }

        [Required]
        public ActivityStatus ActivityStatus { get; set; }
    }
}
