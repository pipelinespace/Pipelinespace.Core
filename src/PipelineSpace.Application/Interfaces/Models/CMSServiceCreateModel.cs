using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSServiceCreateModel
    {
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string ProjectExternalId { get; set; }
        public bool IsPublic { get; set; }
        public string TeamId { get; set; }

        public static class Factory
        {
            public static CMSServiceCreateModel Create(string teamId, string projectExternalId, string projectName, string name, bool isPublic)
            {
                var entity = new CMSServiceCreateModel()
                {
                    ProjectExternalId = projectExternalId,
                    ProjectName = projectName,
                    Name = name,
                    TeamId = teamId,
                    IsPublic = isPublic
                };
                return entity;
            }
        }
    }

    public class CMSServiceCreateResultModel : CMSBaseResultModel
    {
        public string ServiceExternalId { get; set; }
        public string ServiceExternalUrl { get; set; }
    }
}
