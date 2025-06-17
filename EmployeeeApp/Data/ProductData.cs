using EmployeeeApp.Models;
using Humanizer;
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



        public bool PlaceOrder(Order order)
        {

            int rowaffected = 0;
            int insertedOrderId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Insert into Orders (ClientId,AddressId,TotalPrice) output inserted.OrderId values(@ClientId,@AddressId,@TtalPrice); ", connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", order.ClientId);
                        command.Parameters.AddWithValue("@AddressId", order.AddressId);
                        command.Parameters.AddWithValue("@TtalPrice", order.TotalPrice);
                        insertedOrderId = (int)command.ExecuteScalar();
                    }
                    if (insertedOrderId > 0)
                    {

                        if (order.OrderItems.Count() > 0)
                        {
                            foreach (var item in order.OrderItems)
                            {
                                using (SqlCommand command = new SqlCommand("Insert into OrderItems (OrderId,ProductId,TotalpricetoItem,Quantity) values(@OrderId,@ProductId,@TotalpriceToitem,@Quantity)", connection))
                                {
                                    command.Parameters.AddWithValue("@OrderId", insertedOrderId);
                                    command.Parameters.AddWithValue("@ProductId", item.productId);
                                    command.Parameters.AddWithValue("@Quantity", item.quantity);
                                    command.Parameters.AddWithValue("@TotalpriceToitem", item.TotalPrice);
                                    command.ExecuteNonQuery();

                                }


                            }
                        }

                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("No data Inserted :" + ex.Message);
            }


        }


        public List<Order> OrderForId(int id)
        {
            Order order = new Order();
            try
            {
                List<Order> orders = new List<Order>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT C.Id AS ClientId, C.Name AS ClientName, O.OrderId, O.AddressId, O.TotalPrice AS OrderTotalPrice, " +
                        "CD.CityName, CD.StateName, CD.pincode, " +
                        "OI.OrderItemId, OI.ProductId, OI.TotalpricetoItem AS OrderItemUnitPrice, OI.Quantity, OI.TotalpricetoItem AS OrderItemTotalPrice " +
                        "FROM [Employee].[dbo].[Orders] AS O " +
                        "LEFT JOIN [Employee].[dbo].[Client] AS C ON O.ClientId = C.Id " +
                        "LEFT JOIN [Employee].[dbo].[ClientDetails] AS CD ON O.AddressId = CD.AddressId AND O.ClientId = CD.ClientId " +
                        "LEFT JOIN [Employee].[dbo].[OrderItems] AS OI ON O.OrderId = OI.OrderId " +
                        "WHERE O.ClientId = @ClientId " +
                        "ORDER BY O.OrderId DESC;", connection))
                    {

                        command.Parameters.AddWithValue("@ClientId", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int currentOrderId = Convert.ToInt32(reader["OrderId"]);


                                Order existingOrder = orders.FirstOrDefault(o => o.OrderId == currentOrderId);

                                if (existingOrder == null)
                                {

                                    existingOrder = new Order
                                    {
                                        OrderId = currentOrderId,
                                        ClientId = Convert.ToInt32(reader["ClientId"]),
                                        ClientName = Convert.ToString(reader["ClientName"]),
                                        AddressId = Convert.ToInt32(reader["AddressId"]),
                                        AddressPlace = $"{reader["CityName"]}, {reader["StateName"]}, {reader["pincode"]}",
                                        TotalPrice = Convert.ToDecimal(reader["OrderTotalPrice"]),
                                        OrderItems = new List<OrderItems>()
                                    };
                                    orders.Add(existingOrder); 
                                }

                                
                                if (reader["OrderItemId"] != DBNull.Value)
                                {
                                    OrderItems item = new OrderItems
                                    {
                                        OrderItemId = Convert.ToInt32(reader["OrderItemId"]),
                                        OrderId = currentOrderId,
                                        productId = Convert.ToInt32(reader["ProductId"]),
                                        Unitprice = Convert.ToDecimal(reader["OrderItemUnitPrice"]),
                                        quantity = Convert.ToInt32(reader["Quantity"]),
                                        TotalPrice = Convert.ToDecimal(reader["OrderItemTotalPrice"])
                                    };
                                    existingOrder.OrderItems.Add(item); 
                                }
                            }
                        }
                    }
                }
                return orders;



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public bool RemoveOrder(int Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("delete from Orders where OrderId = @Id ; ", connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        Object result = command.ExecuteNonQuery();
                        int row = (int)result;
                        return row > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in OrderCancellation : " + ex.Message);
            }

        }

        public Orderlist GetOrder(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    Orderlist orderlist = new Orderlist();
                    orderlist.order = new Order
                    {
                        OrderId = orderId,
                        OrderItems = new List<OrderItems>() 
                    };

                    using (SqlCommand command = new SqlCommand(
                        "SELECT C.Id AS ClientId, C.Name AS ClientName, O.OrderId, O.AddressId, O.TotalPrice AS OrderTotalPrice, " +
                        "CD.CityName, CD.StateName, CD.pincode, " +
                        "OI.OrderItemId, OI.ProductId, OI.TotalpricetoItem AS OrderItemUnitPrice, OI.Quantity, OI.TotalpricetoItem AS OrderItemTotalPrice " +
                        "FROM [Employee].[dbo].[Orders] AS O " +
                        "LEFT JOIN [Employee].[dbo].[Client] AS C ON O.ClientId = C.Id " +
                        "LEFT JOIN [Employee].[dbo].[ClientDetails] AS CD ON O.AddressId = CD.AddressId AND O.ClientId = CD.ClientId " +
                        "LEFT JOIN [Employee].[dbo].[OrderItems] AS OI ON O.OrderId = OI.OrderId " +
                        "WHERE O.OrderId = @OrderId " +
                        "ORDER BY O.OrderId DESC;", connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                if (string.IsNullOrEmpty(orderlist.Name))
                                {
                                    orderlist.Name = reader["ClientName"].ToString();
                                    orderlist.order.ClientId = Convert.ToInt32(reader["ClientId"]);
                                    orderlist.order.ClientName = reader["ClientName"].ToString();
                                    orderlist.order.AddressId = Convert.ToInt32(reader["AddressId"]);
                                    orderlist.order.AddressPlace = $"{reader["CityName"]}, {reader["StateName"]}, {reader["Pincode"]}";
                                    orderlist.order.TotalPrice = Convert.ToDecimal(reader["OrderTotalPrice"]);
                                }

                               
                                if (!reader.IsDBNull(reader.GetOrdinal("OrderItemId")))
                                {
                                    var item = new OrderItems
                                    {
                                        OrderItemId = Convert.ToInt32(reader["OrderItemId"]),
                                        OrderId = orderId,
                                        productId = Convert.ToInt32(reader["ProductId"]),
                                        Unitprice = Convert.ToDecimal(reader["OrderItemUnitPrice"]),
                                        quantity = Convert.ToInt32(reader["Quantity"]),
                                        TotalPrice = Convert.ToDecimal(reader["OrderItemTotalPrice"])
                                    };

                                    orderlist.order.OrderItems.Add(item);
                                }
                            }
                        }
                    }

                    return orderlist;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot get the order details: " + ex.Message);
            }
        }


    }
}
