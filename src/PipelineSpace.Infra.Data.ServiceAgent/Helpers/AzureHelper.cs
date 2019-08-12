using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Infra.Data.ServiceAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Helpers
{
    public static class AzureHelper
    {
        public static PipelineEnvironmentStatusModel GetStatusModel(string status)
        {
            PipelineEnvironmentStatusModel model = new PipelineEnvironmentStatusModel();
            model.StatusCode = PipelineEnvironmentStatusEnumModel.Pending.ToString();
            model.StatusName = PipelineEnvironmentStatusEnumModel.Pending.GetDescription();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("Deploying", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.InProgress.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.InProgress.GetDescription();
                }

                if (status.Equals("Succeeded", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.Succeeded.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.Succeeded.GetDescription();
                }

                if (status.Equals("Failed", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.Succeeded.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.Succeeded.GetDescription();
                }

                if (status.Equals("Canceled", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.Canceled.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.Canceled.GetDescription();
                }
            }
            
            return model;
        }
    }
}
