using Final_Project.Models;
using System.ComponentModel.DataAnnotations;

public class SaleDateAfterHireDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var salesData = (SalesData)validationContext.ObjectInstance;
        var employee = salesData.Employee;

        // If we don't have an employee or hire date, skip validation
        if (employee?.DateOfHire == null)
            return ValidationResult.Success;

        // Get the sale date (year/quarter converted to date)
        var saleDate = GetSaleDate(salesData.Year, salesData.Quarter);

        if (saleDate < employee.DateOfHire)
        {
            return new ValidationResult($"Sales cannot be created before employee's hire date: {employee.DateOfHire:d}");
        }

        return ValidationResult.Success;
    }

    private DateTime GetSaleDate(int year, int quarter)
    {
        int month = (quarter - 1) * 3 + 1;
        return new DateTime(year, month, 1);
    }
}