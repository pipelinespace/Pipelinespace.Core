using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Models
{
    public class RealtimePostRp
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ActivityName { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
