using System.Diagnostics.CodeAnalysis;

namespace EmployeeeApp.Models
{
    public class ClientViewID
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Role { get; set; }

        public string? Email { get; set; }

        public string? AddressJson { get; set; }

        public List<string> Addresses = new List<string>();
       
    }
}
