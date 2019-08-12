using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectFeatureServiceDeliveryListRp
    {
        public ProjectFeatureServiceDeliveryListRp()
        {
            Items = new List<ProjectFeatureServiceDeliveryListItemRp>();
        }

        public IReadOnlyList<ProjectFeatureServiceDeliveryListItemRp> Items { get; set; }
    }

    public class ProjectFeatureServiceDeliveryListItemRp
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ProjectServiceDeliveryDataRp Data { get; set; }
    }

    public class ProjectFeatureServiceDeliveryDataRp
    {
        public string BuildStatus { get; set; }
        public List<ProjectFeatureServiceDeliveryDataEnvironmentModel> Environments { get; set; }
    }

    public class ProjectFeatureServiceDeliveryDataEnvironmentModel
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
    }
}
