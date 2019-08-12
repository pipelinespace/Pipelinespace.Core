using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectUserInvitation : BaseEntity
    {
        [Required]
        public Guid ProjectUserInvitationId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }
        
        [StringLength(450)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public PipelineRole Role { get; set; }

        [Required]
        public UserInvitationType InvitationType { get; set; }

        [Required]
        public UserInvitationStatus InvitationStatus { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public void Cancel()
        {
            if (this.InvitationStatus == UserInvitationStatus.Pending)
            {
                this.InvitationStatus = UserInvitationStatus.Canceled;
            }
        }

        public void Accept()
        {
            if (this.InvitationStatus == UserInvitationStatus.Pending)
            {
                this.InvitationStatus = UserInvitationStatus.Accepted;
                this.AcceptedDate = DateTime.UtcNow;
            }
        }

        public static class Factory
        {
            public static ProjectUserInvitation Create(Guid projectId, string userId, string userEmail, PipelineRole role, string createdBy)
            {
                var entity = new ProjectUserInvitation()
                {
                    ProjectId = projectId,
                    UserId = userId,
                    UserEmail = userEmail,
                    InvitationType = string.IsNullOrEmpty(userId) ? UserInvitationType.ExternalUser : UserInvitationType.InternalUser,
                    Role = role,
                    InvitationStatus = UserInvitationStatus.Pending,
                    CreatedBy = createdBy,
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectUserInvitation>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
