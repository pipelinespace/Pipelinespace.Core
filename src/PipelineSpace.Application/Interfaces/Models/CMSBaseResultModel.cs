using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces.Models
{
    public abstract class CMSBaseResultModel
    {
        public CMSBaseResultModel()
        {
            this.Success = true;
        }

        public bool Success { get; set; }

        private string reasonForNoSuccess;

        public string GetReasonForNoSuccess()
        {
            return reasonForNoSuccess;
        }

        private void SetReasonForNoSuccess(string value)
        {
            reasonForNoSuccess = value;
        }

        public void Fail(string reason)
        {
            this.Success = false;
            this.SetReasonForNoSuccess(reason);
        }

    }
}
