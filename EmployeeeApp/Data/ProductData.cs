using Microsoft.Data.SqlClient;
using System.Configuration;

namespace EmployeeeApp.Data
{
    public class ProductData
    {

        private readonly string _connectionString;

        public ProductData()
        {
            _connectionString = GetConnectionString();
        }

      
        public IConfiguration Configuration { get; set; }

        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetConnectionString("DefaultConnection");
        }


        public List<Models.Product> GetProducts()
        {
            var products = new List<Models.Product>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Products", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Models.Product
                        {
                            ProductId = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            ProductPrice = reader.GetDecimal(2),
                          
                            CategoryId = reader.GetInt32(3)
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }
    }
}
