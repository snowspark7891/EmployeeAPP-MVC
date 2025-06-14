using System.ComponentModel.DataAnnotations;

namespace EmployeeeApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage="You must required an address to place order")]
       public int AddressId { get; set; }


        public List<OrderItems> OrderItems = new List<OrderItems>();

        public decimal TotalPrice { get; set; }
    }
}
