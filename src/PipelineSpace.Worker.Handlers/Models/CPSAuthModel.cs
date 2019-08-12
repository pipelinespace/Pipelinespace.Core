using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Handlers.Models
{
    public class CPSAuthModel
    {
        public string AccessId { get; set; }

        public string AccessName { get; set; }

        public string AccessSecret { get; set; }

        public string AccessRegion { get; set; }

        public string AccessAppId { get; set; }
        public string AccessAppSecret { get; set; }
        public string AccessDirectory { get; set; }

    }
}
