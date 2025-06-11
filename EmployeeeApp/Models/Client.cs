using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class Client
    {
        [Key, Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
      
        public string? Role { get; set; }
        
        public string? Email { get; set; }
        
    }
}
