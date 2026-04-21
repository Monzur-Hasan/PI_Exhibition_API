using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Domain.Shared.ValidationFilters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfValueAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        private string[] checkingValues { get; set; }
        public RequiredIfValueAttribute(string propertyName, string[] values)
        {
            PropertyName = propertyName;
            checkingValues = values;
        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            var isExistInValues = checkingValues.FirstOrDefault(s => s == type.GetProperty(PropertyName).GetValue(instance)?.ToString());
            //|| value?.ToString() =="0"
            if (isExistInValues != null && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
