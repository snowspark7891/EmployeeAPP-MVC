namespace EmployeeeApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

       
        public int CategoryId { get; set; }

        public int? ProductQuntity { get; set; }
        public int?  ProductTotal { get; set; }   

    }
}
