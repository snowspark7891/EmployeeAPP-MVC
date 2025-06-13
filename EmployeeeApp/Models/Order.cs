namespace EmployeeeApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }


        public List<ClientDetails> Addresses = new List<ClientDetails>();

        public List<Product> Products = new List<Product>();
      
       public decimal TotalPrice { get; set; }
    }
}
