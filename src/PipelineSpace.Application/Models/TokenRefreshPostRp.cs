using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class TokenRefreshPostRp
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
