using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class ClientView
    {
        public int Id { get; set; }
        public string Name { get; set; }

      
        [EmailAddress(ErrorMessage = "Invalid email address format.")] 
        [RegularExpression(@"^[a-z0-9._-]+@[a-z0_9._-]+\.[a-z]{2,}$", ErrorMessage = "Email must be a valid Email address witrh domain name  (e.g., something23@gmail.com).")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public string? Role { get; set; }
        public List<ClientDetails> Clientlist { get; set; } = new List<ClientDetails>();


    }

  
}
