using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class ClientDetails
    {
        [Key]
        public int OrderId { get; set; }  
        public int ClientId { get; set; }

        public string? Address { get; set; } = string.Empty;
    }
}
