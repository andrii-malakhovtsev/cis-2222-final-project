using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class HireDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var employee = (Employee)validationContext.ObjectInstance;
            var hireDate = (DateTime)value;

            // Check after 1995
            if (hireDate < new DateTime(1995, 1, 1))
            {
                return new ValidationResult("Hire date must not be before 1/1/1995.");
            }

            if (hireDate < employee.DOB)
            {
                return new ValidationResult("Hire date must be after date of birth.");
            }

            return ValidationResult.Success;
        }
    }
}