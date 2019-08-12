using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Options
{
    public class ApplicationOptions
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}
