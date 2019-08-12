using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class ProjectCloudCredentialModel
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccessSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccountProjectId { get; set; }
        public string ProjectName { get; set; }
        public ConfigurationManagementService CMSType { get; set; }
        public string ProjectExternalId { get; set; }
        public string AccessId { get; set; }
        public string RepositoryAccessId { get; set; }
        public string RepositoryCMSType { get; set; }
        public string RepositoryAccessSecret { get; set; }
        public string RepositoryAccessToken { get; set; }
        public string BranchUrl { get; set; }
        public CMSAuthCredentialModel CMSAuthCredential { get; set; }
    }
}
