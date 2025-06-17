namespace EmployeeeApp.Models
{
    public class Orderlist
    {
        public int Id { get; set; } 
        public string Name { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }

        public Order order { get; set; }


    }
}
