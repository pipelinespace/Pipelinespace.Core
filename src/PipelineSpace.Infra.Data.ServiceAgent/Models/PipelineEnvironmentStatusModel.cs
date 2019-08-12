using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models
{
    public class PipelineEnvironmentStatusModel
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
    }

    public enum PipelineEnvironmentStatusEnumModel
    {
        [Description("Pending")]
        Pending = 0,
        [Description("In Progress")]
        InProgress = 1,
        [Description("Succeeded")]
        Succeeded = 2,
        [Description("Failed")]
        Failed = 3,
        [Description("Canceled")]
        Canceled = 4
    }
}
