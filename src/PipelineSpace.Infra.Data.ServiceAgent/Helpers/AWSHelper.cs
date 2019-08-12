using Amazon.CloudFormation;
using PipelineSpace.Infra.CrossCutting.Extensions;
using PipelineSpace.Infra.Data.ServiceAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Helpers
{
    public static class AWSHelper
    {
        public static PipelineEnvironmentStatusModel GetStatusModel(string status)
        {
            PipelineEnvironmentStatusModel model = new PipelineEnvironmentStatusModel();
            model.StatusCode = PipelineEnvironmentStatusEnumModel.Pending.ToString();
            model.StatusName = PipelineEnvironmentStatusEnumModel.Pending.GetDescription();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals(StackStatus.REVIEW_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.CREATE_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.DELETE_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.ROLLBACK_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.UPDATE_COMPLETE_CLEANUP_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.UPDATE_ROLLBACK_COMPLETE_CLEANUP_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                status.Equals(StackStatus.UPDATE_ROLLBACK_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.InProgress.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.InProgress.GetDescription();
                }

                if (status.Equals(StackStatus.CREATE_COMPLETE.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.DELETE_COMPLETE.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.ROLLBACK_COMPLETE.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.UPDATE_COMPLETE.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.UPDATE_COMPLETE_CLEANUP_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.UPDATE_ROLLBACK_COMPLETE.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.UPDATE_ROLLBACK_COMPLETE_CLEANUP_IN_PROGRESS.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.Succeeded.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.Succeeded.GetDescription();
                }

                if (status.Equals(StackStatus.CREATE_FAILED.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.DELETE_FAILED.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.ROLLBACK_FAILED.Value, StringComparison.InvariantCultureIgnoreCase) ||
                    status.Equals(StackStatus.UPDATE_ROLLBACK_FAILED.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.StatusCode = PipelineEnvironmentStatusEnumModel.Failed.ToString();
                    model.StatusName = PipelineEnvironmentStatusEnumModel.Failed.GetDescription();
                }
            }
            
            return model;
        }
    }
}
