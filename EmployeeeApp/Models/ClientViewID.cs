using System.Diagnostics.CodeAnalysis;

namespace EmployeeeApp.Models
{
    public class ClientViewID
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        [AllowNull]
        public List<ClientDetails> list = new List<ClientDetails>();
       
    }
}
