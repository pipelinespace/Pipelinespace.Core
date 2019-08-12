using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public class CMSAuthCredentialModel
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string AccountId { get; set; }
        public string AccessToken { get; set; }
        public string Provider { get; set; }
    }
}
