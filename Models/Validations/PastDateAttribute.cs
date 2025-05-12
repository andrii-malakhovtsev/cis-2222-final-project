using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.Today) // Maybe additional day for time zone issues and current transactions
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be in the past.");
                }
            }
            return ValidationResult.Success;
        }
    }
}