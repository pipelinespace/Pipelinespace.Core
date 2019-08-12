using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            CreationDate = DateTime.UtcNow;
            Status = EntityStatus.Preparing;
        }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(450)]
        public string CreatedBy { get; set; }

        public DateTime? LastModificationDate { get; set; }

        [StringLength(450)]
        public string ModifiedBy { get; set; }

        public DateTime? InactivationDate { get; set; }

        [StringLength(450)]
        public string InactivatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        [StringLength(450)]
        public string DeletedBy { get; set; }
        
        [Required]
        public EntityStatus Status { get; set; }

        public void Activate()
        {
            if (this.Status == EntityStatus.Preparing)
            {
                this.Status = EntityStatus.Active;
            }
        }

        public void Rectivate()
        {
            if (this.Status == EntityStatus.Inactive)
            {
                this.Status = EntityStatus.Active;
            }
        }

        public void Inactivate(string userId)
        {
            if (this.Status == EntityStatus.Active)
            {
                this.InactivatedBy = userId;
                this.InactivationDate = DateTime.UtcNow;
                this.Status = EntityStatus.Inactive;
            }
        }

        public void Delete(string userId)
        {
            if (this.Status == EntityStatus.Active || this.Status == EntityStatus.Inactive)
            {
                this.DeletedBy = userId;
                this.DeletionDate = DateTime.UtcNow;
                this.Status = EntityStatus.Deleted;
            }
        }

        public void Audit(string userId)
        {
            this.ModifiedBy = userId;
            this.LastModificationDate = DateTime.UtcNow;
        }
    }

    public enum EntityStatus
    {
        Preparing = 0,
        Active = 1,
        Inactive = 2,
        Deleted = 3
    }
}
