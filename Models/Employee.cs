using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required!")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required!")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required!")]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of birth must be in the past.")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Date of hire is required!")]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of hire must be in the past.")]
        [HireDateValidation]
        [Display(Name = "Date of Hire")]
        public DateTime DateOfHire { get; set; }

        [Display(Name = "Manager")]
        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public Employee? Manager { get; set; }

        public List<Employee> Subordinates { get; set; } = new();

        public List<SalesData> Sales { get; set; } = new();

        [NotMapped]
        [Display(Name = "Full Name")] // Add optional Middle Name
        public string FullName => $"{Firstname} {Lastname}";
    }
}