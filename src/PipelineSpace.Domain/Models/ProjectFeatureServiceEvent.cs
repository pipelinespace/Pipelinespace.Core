using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectFeatureServiceEvent : BaseEntity
    {
        public Guid ProjectFeatureServiceEventId { get; set; }

        [Required]
        public Guid ProjectFeatureServiceId { get; set; }

        public virtual ProjectFeatureService ProjectFeatureService { get; set; }

        [Required]
        public BaseEventType BaseEventType { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string EventDescription { get; set; }

        [Required]
        public string EventStatus { get; set; }

        [Required]
        public string EventMessage { get; set; }

        [Required]
        public string EventDetailedMessage { get; set; }

        [Required]
        public string EventResource { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public static class Factory
        {
            public static ProjectFeatureServiceEvent Create(BaseEventType baseEventType, string eventType, string eventDescription, string eventStatus, string eventMessage, string eventDetailedMessage, string eventResource, DateTime eventDate)
            {
                var entity = new ProjectFeatureServiceEvent()
                {
                    BaseEventType = baseEventType,
                    EventType = eventType,
                    EventDescription = eventDescription,
                    EventStatus = eventStatus,
                    EventMessage = eventMessage,
                    EventDetailedMessage = eventDetailedMessage,
                    EventResource = eventResource,
                    EventDate = eventDate,
                    CreatedBy = "admin",
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectFeatureServiceEvent>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
