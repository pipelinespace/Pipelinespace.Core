using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Logging.Models
{
    public class EnvyErrorResponse
    {
        public string ErrorId { get; set; }
        public string Message { get; set; }
    }
}
