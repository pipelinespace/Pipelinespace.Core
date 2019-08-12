using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineSpace.Domain.Core.Validators.ValidatorManagers
{
    public class DataValidatorManager<T>
    {
        public List<ValidationResult> Results { get; set; }
        public T obj { get; set; }

        public DataValidatorManager<T> Build()
        {
            return this;
        }

        public DataValidatorManager<T> Validate(T obj)
        {
            this.obj = obj;
            return this;
        }
        
        public bool IsValid
        {
            get
            {
                this.Results = new List<ValidationResult>();
                var context = new ValidationContext(obj);
                return Validator.TryValidateObject(obj, context, Results, true);
            }
        }

        public string Errors
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in Results)
                {
                    sb.AppendLine(item.ErrorMessage);
                }
                return sb.ToString();
            }
        }
    }
}
