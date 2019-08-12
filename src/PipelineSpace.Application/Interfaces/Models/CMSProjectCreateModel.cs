using Newtonsoft.Json;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSProjectCreateModel
    {
        public string TeamId { get; set; }
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectVisibility ProjectVisibility { get; set; }

        public static class Factory
        {
            public static CMSProjectCreateModel Create(string organizationName, string name, string description, ProjectVisibility projectVisibility)
            {
                var entity = new CMSProjectCreateModel()
                {
                    Name = name,
                    OrganizationName = organizationName,
                    Description = description,
                    ProjectVisibility = projectVisibility
                };
                return entity;
            }
        }
    }

    public class CMSProjectCreateResultModel : CMSBaseResultModel
    {
        public string ProjectExternalId { get; set; }
    }
}
