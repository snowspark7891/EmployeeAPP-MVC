using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class Client
    {
        [Key, Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        
    }
}
