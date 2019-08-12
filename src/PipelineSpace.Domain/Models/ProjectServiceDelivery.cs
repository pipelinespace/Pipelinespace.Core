using Newtonsoft.Json;
using PipelineSpace.Domain.Core.Validators.ValidatorManagers;
using PipelineSpace.Domain.ModelUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PipelineSpace.Domain.Models
{
    public class ProjectServiceDelivery : BaseEntity
    {
        public ProjectServiceDelivery()
        {

        }

        public Guid ProjectServiceDeliveryId { get; set; }

        [Required]
        public Guid ProjectServiceId { get; set; }

        public virtual ProjectService ProjectService { get; set; }

        [Required]
        public int VersionId { get; set; }

        [Required]
        public string VersionName { get; set; }

        public string Data { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
       
        private DeliveryModel GetData()
        {
            if (string.IsNullOrEmpty(this.Data))
            {
                return new DeliveryModel();
            }

            return JsonConvert.DeserializeObject<DeliveryModel>(this.Data);
        }

        public void UpdateBuildInformation(string status, DateTime deliveryDate)
        {
            this.DeliveryDate = deliveryDate;

            var deliveryModel = this.GetData();
            deliveryModel.BuildStatus = status;

            if (deliveryModel.Environments == null)
                deliveryModel.Environments = new List<DeliveryEnvironmentModel>();

            this.Data = JsonConvert.SerializeObject(deliveryModel);
        }

        public void AddReleaseStarted(List<DeliveryEnvironmentModel> environments)
        {
            var deliveryModel = this.GetData();
            if(deliveryModel.Environments == null)
            {
                deliveryModel.Environments = new List<DeliveryEnvironmentModel>();
            }

            foreach (var environment in environments)
            {
                var deliveryEnvironment = deliveryModel.Environments.FirstOrDefault(x => x.Name.Equals(environment.Name, StringComparison.InvariantCultureIgnoreCase));
                if(deliveryEnvironment == null)
                {
                    deliveryModel.Environments.Add(new DeliveryEnvironmentModel() {
                        Name = environment.Name,
                        Rank = environment.Rank,
                        Status = environment.Status
                    });
                }
                else
                {
                    deliveryEnvironment.Name = environment.Name;
                    deliveryEnvironment.Rank = environment.Rank;
                    deliveryEnvironment.Status = environment.Status;
                }
            }

            this.Data = JsonConvert.SerializeObject(deliveryModel);
        }

        public void UpdateReleaseStatus(string environmentName, string environmentStatus)
        {
            var deliveryModel = this.GetData();
            if (deliveryModel.Environments == null)
            {
                deliveryModel.Environments = new List<DeliveryEnvironmentModel>();
            }

            var deliveryEnvironment = deliveryModel.Environments.FirstOrDefault(x => x.Name.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase));
            if (deliveryEnvironment != null)
            {
                deliveryEnvironment.Status = environmentStatus;
            }

            this.Data = JsonConvert.SerializeObject(deliveryModel);
        }
        
        public static class Factory
        {
            public static ProjectServiceDelivery Create(int versionId, string versionName)
            {
                var entity = new ProjectServiceDelivery()
                {
                    ProjectServiceDeliveryId = Guid.NewGuid(),
                    VersionId = versionId,
                    VersionName = versionName,
                    CreatedBy = "admin",
                    Status = EntityStatus.Active
                };

                var validationResult = new DataValidatorManager<ProjectServiceDelivery>().Build().Validate(entity);
                if (!validationResult.IsValid)
                    throw new ApplicationException(validationResult.Errors);

                return entity;
            }
        }
    }
}
