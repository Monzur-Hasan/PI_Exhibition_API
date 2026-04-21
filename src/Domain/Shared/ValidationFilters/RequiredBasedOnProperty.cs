using Domain.Shared.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Domain.Shared.ValidationFilters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredBasedOnProperty : RequiredAttribute
    {
        private string _PropertyName { get; set; }
        private string _Comparevalue { get; set; }
        private bool _IfNumeric { get; set; }
        public RequiredBasedOnProperty(string PropertyName, string Comparevalue, bool IfNumeric)
        {
            _PropertyName = PropertyName;
            _Comparevalue = Comparevalue;
            _IfNumeric = IfNumeric;
        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            if ((type.GetProperty(_PropertyName).GetValue(instance)?.ToString()).ToLower() == _Comparevalue?.ToString().ToLower())
            {
                if (_IfNumeric)
                {
                    if (UtilityService.TryParseLong(value?.ToString().ToLower()) <= 0)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
                else
                {
                    if (value?.ToString().ToLower() == null || value?.ToString().ToLower() == "")
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
