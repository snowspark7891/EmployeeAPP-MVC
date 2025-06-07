using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [MaxLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of Joining is required")]
        public DateTime Dateofjoining { get; set; } 

        public string? Profilepic { get; set; }
    }
}