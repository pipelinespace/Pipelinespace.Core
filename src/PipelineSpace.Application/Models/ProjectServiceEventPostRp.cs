using Newtonsoft.Json;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProjectServiceEventPostRp
    {
        public string SubscriptionId { get; set; }
        public int NotificationId { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string PublisherId { get; set; }
        public ProjectServiceEventMessagePostRp Message { get; set; }
        public object DetailedMessage { get; set; }
        public object Resource { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

        public PipelineEventType GetEventType()
        {
            PipelineEventType _eventType = PipelineEventType.BuildStarted;
            if (EventType.Equals("git.push", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.BuildStarted;
            }
            if (EventType.Equals("build.complete", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.BuildCompleted;
            }
            if (EventType.Equals("ms.vss-release.deployment-started-event", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.ReleaseStarted;
            }
            if (EventType.Equals("ms.vss-release.deployment-approval-pending-event", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.ReleasePendingApproval;
            }
            if (EventType.Equals("ms.vss-release.deployment-approval-completed-event", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.ReleaseCompletedApproval;
            }
            if (EventType.Equals("ms.vss-release.deployment-completed-event", StringComparison.InvariantCultureIgnoreCase))
            {
                _eventType = PipelineEventType.ReleaseCompleted;
            }
            return _eventType;
        }
        
        public ProjectServiceEventBuildModel BuildBuildModel()
        {
            string jsonObject = JsonConvert.SerializeObject(this.Resource);
            return JsonConvert.DeserializeObject<ProjectServiceEventBuildModel>(jsonObject);
        }

        public ProjectServiceEventReleaseStartedModel BuildReleaseStartedModel()
        {
            string jsonObject = JsonConvert.SerializeObject(this.Resource);
            return JsonConvert.DeserializeObject<ProjectServiceEventReleaseStartedModel>(jsonObject);
        }

        public ProjectServiceEventReleaseModel BuildReleaseModel()
        {
            string jsonObject = JsonConvert.SerializeObject(this.Resource);
            return JsonConvert.DeserializeObject<ProjectServiceEventReleaseModel>(jsonObject);
        }

        public ProjectServiceEventReleaseApprovalModel BuildReleaseApprovalModel()
        {
            string jsonObject = JsonConvert.SerializeObject(this.Resource);
            return JsonConvert.DeserializeObject<ProjectServiceEventReleaseApprovalModel>(jsonObject);
        }
    }

    public class ProjectServiceEventMessagePostRp
    {
        public string Text { get; set; }
        public string Html { get; set; }
        public string Markdown { get; set; }
    }
}
