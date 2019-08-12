using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CPSCloudResourceSummaryModel
    {
        public CPSCloudResourceSummaryModel()
        {
            Environments = new List<CPSCloudResourceEnvironmentSummaryModel>();
        }

        public List<CPSCloudResourceEnvironmentSummaryModel> Environments { get; set; }

        public CPSCloudResourceEnvironmentSummaryModel AddEnvironment(string name, string status)
        {
            if (Environments == null)
                Environments = new List<CPSCloudResourceEnvironmentSummaryModel>();

            CPSCloudResourceEnvironmentSummaryModel environment = new CPSCloudResourceEnvironmentSummaryModel();
            environment.Name = name;
            environment.StatusName = status;

            Environments.Add(environment);

            return environment;
        }
    }

    public class CPSCloudResourceEnvironmentSummaryModel
    {
        public string Name { get; set; }

        public string StatusCode { get; set; }

        public string StatusName { get; set; }

        public List<CPSCloudResourceEnvironmentPropertiesSummaryModel> Properties { get; set; }
        public List<CPSCloudResourceEnvironmentItemSummaryModel> Resources { get; set; }

        public CPSCloudResourceEnvironmentSummaryModel()
        {
            Properties = new List<CPSCloudResourceEnvironmentPropertiesSummaryModel>();
            Resources = new List<CPSCloudResourceEnvironmentItemSummaryModel>();
        }

        public void AddProperty(string name, string value)
        {
            if (Properties == null)
                Properties = new List<CPSCloudResourceEnvironmentPropertiesSummaryModel>();

            CPSCloudResourceEnvironmentPropertiesSummaryModel property = new CPSCloudResourceEnvironmentPropertiesSummaryModel();
            property.Name = name;
            property.Value = value;

            Properties.Add(property);
        }

        public void AddResource(string type, string id, string name, string status)
        {
            if (Resources == null)
                Resources = new List<CPSCloudResourceEnvironmentItemSummaryModel>();

            CPSCloudResourceEnvironmentItemSummaryModel resource = new CPSCloudResourceEnvironmentItemSummaryModel();
            resource.Id = id;
            resource.Status = status;
            resource.Name = name;
            resource.Type = type;

            Resources.Add(resource);
        }
    }

    public class CPSCloudResourceEnvironmentPropertiesSummaryModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class CPSCloudResourceEnvironmentItemSummaryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
    


}
