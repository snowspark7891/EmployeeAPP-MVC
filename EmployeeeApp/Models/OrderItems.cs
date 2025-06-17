namespace EmployeeeApp.Models
{
    public class OrderItems
    {
        public int OrderItemId { get; set; }
        
        public int OrderId { get; set; }

        public int productId { get; set; }


       
       public decimal? Unitprice { get; set; }
     
        public int quantity { get; set; }

        public decimal  TotalPrice { get; set; }


    }
}
