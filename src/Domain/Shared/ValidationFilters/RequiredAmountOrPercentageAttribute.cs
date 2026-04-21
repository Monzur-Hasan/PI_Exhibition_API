using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Shared.ValidationFilters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAmountOrPercentageAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        public RequiredAmountOrPercentageAttribute(string comparePropertyName)
        {
            PropertyName = comparePropertyName;

        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            if ((type.GetProperty(PropertyName).GetValue(instance)?.ToString()).ToLower() == value?.ToString().ToLower())
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
