namespace EmployeeeApp.Models
{
    public class ClientDetails
    {
        public int OrderId { get; set; }  
        public int ClientId { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
