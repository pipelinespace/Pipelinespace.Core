using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceDeliveryListRp
    {
        public ProjectServiceDeliveryListRp()
        {
            Items = new List<ProjectServiceDeliveryListItemRp>();
        }

        public IReadOnlyList<ProjectServiceDeliveryListItemRp> Items { get; set; }
    }

    public class ProjectServiceDeliveryListItemRp
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ProjectServiceDeliveryDataRp Data { get; set; }
    }

    public class ProjectServiceDeliveryDataRp
    {
        public string BuildStatus { get; set; }
        public List<ProjectServiceDeliveryDataEnvironmentModel> Environments { get; set; }
    }

    public class ProjectServiceDeliveryDataEnvironmentModel
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
    }
}
