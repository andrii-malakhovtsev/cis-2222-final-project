using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class HireDateAfter1995Attribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date < new DateTime(1995, 1, 1)) // Day of company foundation
                {
                    return new ValidationResult(ErrorMessage ?? "Hire date must not be before 1/1/1995.");
                }
            }
            return ValidationResult.Success;
        }
    }
}