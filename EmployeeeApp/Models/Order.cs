using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

     
       public int AddressId { get; set; }

        public  string? AddressPlace { get; set; }
        public List<OrderItems> OrderItems { get; set;} = new List<OrderItems>();

        public decimal TotalPrice { get; set; }
    }
}
