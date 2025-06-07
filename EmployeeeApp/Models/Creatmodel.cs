namespace EmployeeeApp.Models
{
    public class Creatmodel
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string AddressJson { get; set; }= string.Empty;

        public List<string> Addresses
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AddressJson))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(AddressJson);
                }
                return new List<string>();
            }
            set
            {
                AddressJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
         
        } 
    }
}
