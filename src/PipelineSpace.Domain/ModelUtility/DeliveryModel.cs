using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.ModelUtility
{
    public class DeliveryModel
    {
        public string BuildStatus { get; set; }
        public List<DeliveryEnvironmentModel> Environments { get; set; }
    }

    public class DeliveryEnvironmentModel
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
    }
}
