namespace EmployeeeApp.Models
{
    public class ClientView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        
        public string? Address { get; set; }

        public int? addressId { get; set; }

        public List<string> Addresses { get; set; } = new List<string>();


    }
}
