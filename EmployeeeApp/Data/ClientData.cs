using EmployeeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Net;

namespace EmployeeeApp.Data
{
    public class ClientData
    {

        private readonly string _connectionString;
        public static IConfiguration Configuration { get; set; }
        public ClientData()
        {
            _connectionString = GetConnectionString();
        }
        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetConnectionString("DefaultConnection");
        }



        public List<ClientView> GetALL()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Select * from Client order by Id", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            List<ClientView> clients = new List<ClientView>();
                            while (reader.Read())
                            {
                                ClientView client = new ClientView
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : Convert.ToString(reader["Role"]),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : Convert.ToString(reader["Email"])
                                };
                                clients.Add(client);
                            }
                            return clients;
                        }
                    }
                        
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error getting all clients: " + e.Message);
            }
        }



        public bool AddClient(ClientView client)
        {
            try
            {
                int InsertedId = 0;
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Insert into Client (Name,Role,Email) output inserted.Id values(@Name,@Role,@Email)", con))
                    {
                        cmd.Parameters.AddWithValue("@Name", client.Name);
                        cmd.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(client.Role) ? (object)DBNull.Value : client.Role);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(client.Email) ? (object)DBNull.Value : client.Email);
                        Object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            InsertedId = Convert.ToInt32(result);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (InsertedId > 0)
                    {
                        foreach (string address in client.Addresses)
                        {
                            string addressToInsert = string.IsNullOrEmpty(address) ? string.Empty : address;
                            using (SqlCommand command = new SqlCommand("Insert into ClientDetails (ClientId,Address) values (@ClientId , @Address)", con))
                            {
                                command.Parameters.Add("@ClientId", SqlDbType.Int).Value = InsertedId;
                                command.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = addressToInsert;
                                command.ExecuteNonQuery();
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
            catch (Exception e)
            {
                throw new Exception("Error adding client: " + e.Message);
            }
        }



    }

}


//public List<Client> GetAll(int Page = 1, int PageNumber = 10)
//{
//    List<Client> clients = new List<Client>();
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("SELECT * FROM Client ORDER BY Id ASC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
//            {
//                command.Parameters.AddWithValue("@Offset", (Page - 1) * PageNumber);
//                command.Parameters.AddWithValue("@PageSize", PageNumber);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        clients.Add(new Client
//                        {
//                            Id = Convert.ToInt32(reader["Id"]),
//                            Name = Convert.ToString(reader["Name"]),
//                            Role = Convert.ToString(reader["Role"]),
//                            Email = Convert.ToString(reader["Email"])
//                        });
//                    }
//                }
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error retrieving client data: " + ex.Message);
//    }
//    return clients;

//}



//internal int GetClientTotalCount()
//{
//    int count = 0;
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Client", connection))
//            {
//                count = (int)command.ExecuteScalar();
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error retrieving total client count: " + ex.Message);
//    }

//    return count;
//}


//public bool Insert(Client client)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("INSERT INTO Client (Name, Role, Email) VALUES (@Name, @Role, @Email)", connection))
//            {
//                command.Parameters.AddWithValue("@Name", client.Name);
//                command.Parameters.AddWithValue("@Role", client.Role);
//                command.Parameters.AddWithValue("@Email", client.Email);
//                int rowsAffected = command.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error inserting client data: " + ex.Message);
//    }
//}



//public bool Delete(int id)
//{
//    try
//    {
//        int rowaff = 0;
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("DELETE FROM Client WHERE Id = @Id", connection))
//            {
//                command.Parameters.AddWithValue("@Id", id);
//                Object result = (int)command.ExecuteScalar();
//                if (result != null)
//                {
//                    rowaff = Convert.ToInt32(result);

//                }
//                else
//                {
//                    return false;
//                }
//            }
//            if (rowaff > 0)
//            {
//                using (SqlCommand command = new SqlCommand("Delete from ClientDetails where ClientId = @Id", connection))
//                {
//                    command.Parameters.AddWithValue("@Id", id);
//                    command.BeginExecuteNonQuery();
//                }
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error deleting client data: " + ex.Message);
//    }
//}



////public bool InsertAll(Creatmodel ve)
////{
////    try
////    {
////        using (SqlConnection connection = new SqlConnection(_connectionString))
////        {

////            int insertedId = 0;
////            connection.Open();
////            using (SqlCommand command = new SqlCommand("Insert into Client (Name,Role,Email) output inserted.Id values(@Name,@Role,@Email)", connection))
////            {
////                command.Parameters.AddWithValue("@Name", ve.Name);
////                command.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(ve.Role) ? (object)DBNull.Value : ve.Role);
////                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(ve.Email) ? (object)DBNull.Value : ve.Email);
////                Object result = command.ExecuteScalar();
////                if (result != null)
////                {
////                    insertedId = Convert.ToInt32(result);

////                }
////                else
////                {
////                    return false;
////                }
////            }

////            if (insertedId > 0)
////            {

////                foreach (string address in ve.Addresses)
////                {
////                    string addressToInsert = string.IsNullOrEmpty(address) ? string.Empty : address;
////                    using (SqlCommand command = new SqlCommand("Insert into ClientDetails (ClientId,Address) values (@ClientId , @Address)", connection))
////                    {

////                        command.Parameters.Add("@ClientId", SqlDbType.Int).Value = insertedId;
////                        command.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = address;
////                        command.ExecuteNonQuery();
////                    }

////                }
////                return true;
////            }
////            else
////            {
////                return false;
////            }



////        }
////    }
////    catch (Exception ex)
////    {
////        throw new Exception("Error deleting all clients: " + ex.Message);
////    }
////}

//public bool InsertAll(ClientViewID ve)
//{
//    using (SqlConnection connection = new SqlConnection(_connectionString))
//    {
//        connection.Open();
//        SqlTransaction transaction = connection.BeginTransaction();

//        try
//        {
//            int insertedClientId = 0;

//            using (SqlCommand command = new SqlCommand("INSERT INTO Client (Name, Role, Email) OUTPUT INSERTED.Id VALUES (@Name, @Role, @Email)", connection, transaction))
//            {
//                command.Parameters.AddWithValue("@Name", ve.Name);
//                command.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(ve.Role) ? (object)DBNull.Value : ve.Role);
//                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(ve.Email) ? (object)DBNull.Value : ve.Email);

//                Object result = command.ExecuteScalar();
//                if (result != null)
//                {
//                    insertedClientId = (int)result;
//                }
//                else
//                {
//                    transaction.Rollback();
//                    return false;
//                }
//            }

//            if (insertedClientId > 0)
//            {
//                if (ve.Addresses != null && ve.Addresses.Any())
//                {
//                    foreach (ClientDetails addressDetail in ve.Addresses)
//                    {
//                        string addressToInsert = addressDetail.Address;

//                        using (SqlCommand command = new SqlCommand("INSERT INTO ClientDetails (ClientId, Address) VALUES (@ClientId , @Address)", connection, transaction))
//                        {
//                            command.Parameters.Add("@ClientId", SqlDbType.Int).Value = insertedClientId;
//                            command.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = addressToInsert;
//                            command.ExecuteNonQuery();
//                        }
//                    }
//                }
//                transaction.Commit();
//                return true;
//            }
//            else
//            {
//                transaction.Rollback();
//                return false;
//            }
//        }
//        catch (Exception ex)
//        {
//            transaction.Rollback();
//            throw new Exception("An error occurred during the client and details insertion process.", ex);
//        }
//    }
//}

//public ClientViewID GetViewBYId(int Id)
//{
//    ClientViewID client = null;
//    try
//    {

//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("SELECT c.Id, c.Name, C.Role, c.Email, cd.Address, cd.Id AS OrderId FROM Client c left JOIN ClientDetails cd ON c.Id = cd.CLientId WHERE c.Id = @Id ORDER BY c.Id, cd.Address;", connection))
//            {
//                command.Parameters.AddWithValue("@Id", Id);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    bool clientInitial = false;
//                    while (reader.Read())
//                    {
//                        if (!clientInitial)
//                        {
//                            client = new ClientViewID
//                            {
//                                Id = Convert.ToInt32(reader["Id"]),
//                                Name = Convert.ToString(reader["Name"]),
//                                Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : Convert.ToString(reader["Role"]),
//                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : Convert.ToString(reader["Email"]),
//                                //list = new List<ClientDetails>()
//                            };
//                            clientInitial = true;
//                        }
//                        client.list.Add(new ClientDetails
//                        {
//                            //OrderId = Convert.ToInt32(reader["OrderId"]),
//                            OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId")) ? 0 : Convert.ToInt32(reader["OrderId"]),
//                            ClientId = Convert.ToInt32(reader["Id"]),
//                            //Address = Convert.ToString(reader["Address"])
//                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : Convert.ToString(reader["Address"])
//                        });
//                    }
//                }
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error retrieving client by ID: " + ex.Message);
//    }


//    return client;

//}


//public bool UpdateClient(Client clientr)
//{
//    try
//    {

//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("Update Client set  Name = @Name , Role = @Role ,Email = @Email where Id = @Id;", connection))
//            {
//                command.Parameters.AddWithValue("@Id", clientr.Id);
//                command.Parameters.AddWithValue("@Name", clientr.Name);
//                command.Parameters.AddWithValue("@Role", clientr.Role);
//                command.Parameters.AddWithValue("@Email", clientr.Email);
//                int rowwaffected = (int)command.ExecuteNonQuery();
//                if (rowwaffected <= 0)
//                {
//                    return false;
//                }
//            }

//            return true;
//        }


//    }
//    catch (Exception e)
//    {
//        throw new Exception("Error Can't Update Data : " + e.Message);
//    }
//}

//public bool DeleteClient(int Id)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("DELETE FROM Client WHERE Id = @Id", connection))
//            {
//                command.Parameters.AddWithValue("@Id", Id);
//                int rowsAffected = command.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error deleting client: " + ex.Message);
//    }
//}



//public ClientDetails EditDetails(int Id, int OrderId)
//{
//    ClientDetails clientDetails = null;
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("SELECT * FROM ClientDetails WHERE Id = @OrderId AND ClientId = @Id", connection))
//            {
//                command.Parameters.AddWithValue("@OrderId", OrderId);
//                command.Parameters.AddWithValue("@Id", Id);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        clientDetails = new ClientDetails
//                        {
//                            OrderId = Convert.ToInt32(reader["Id"]),
//                            ClientId = Convert.ToInt32(reader["ClientId"]),
//                            Address = Convert.ToString(reader["Address"])
//                        };
//                    }
//                }
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error retrieving client details: " + ex.Message);
//    }
//    return clientDetails;


//}



//public bool AddAddress(ClientDetails clientDetails)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("Update ClientDetails set Address = @Address where Id = @OrderId", connection))
//            {
//                command.Parameters.AddWithValue("@OrderId", clientDetails.OrderId);
//                command.Parameters.AddWithValue("@Address", clientDetails.Address);
//                int rowsAffected = command.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error adding address: " + ex.Message);
//    }
//}

//public bool DeleteAddress(int Id, int OrderId)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("DELETE FROM ClientDetails WHERE Id = @OrderId AND ClientId = @Id", connection))
//            {
//                command.Parameters.AddWithValue("@OrderId", OrderId);
//                command.Parameters.AddWithValue("@Id", Id);
//                int rowsAffected = command.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error deleting address: " + ex.Message);
//    }
//}


//public bool AddAddress(int Id, string address)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            using (SqlCommand command = new SqlCommand("INSERT INTO ClientDetails (ClientId, Address) VALUES (@ClientId, @Address)", connection))
//            {
//                command.Parameters.AddWithValue("@ClientId", Id);
//                command.Parameters.AddWithValue("@Address", address);
//                int rowsAffected = command.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error adding address: " + ex.Message);
//    }
//}


//public static List<string> List = new List<string>();

//public List<string> getList()
//{
//    return List;
//}

//public List<string> AddList(string ad)
//{
//    if (ad != null)
//    {
//        List.Add(ad);
//    }
//    return List;
//}

//public bool DeleteAdd(int ad)
//{
//    int a = 1;
//    if (ad >= 0 && ad <= List.Count())
//    {
//        List.RemoveAt(ad);
//        return a > 0;
//    }
//    return a < 0;
//}

//public bool ListToDB(List<Client> clients)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            connection.Open();
//            foreach (var client in clients)
//            {
//                using (SqlCommand command = new SqlCommand("INSERT INTO Client (Name, Role, Email) VALUES (@Name, @Role, @Email)", connection))
//                {
//                    command.Parameters.AddWithValue("@Name", client.Name);
//                    command.Parameters.AddWithValue("@Role", client.Role);
//                    command.Parameters.AddWithValue("@Email", client.Email);
//                    command.ExecuteNonQuery();
//                }
//            }
//        }
//        return true;
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error inserting list of clients to database: " + ex.Message);
//    }
//}






//public ClientViewID GetViewBYID(int Id, int OrderID)
//{
//    ClientViewID cli = null;

//    try
//    {
//        using (SqlConnection con = new SqlConnection(_connectionString))
//        {
//            con.Open();

//            using (SqlCommand command = new SqlCommand("select c.Id,c.Name,C.Role,c.Email, cd.Address ,cd.Id As OrderId  from Client c join ClientDetails cd on c.Id = cd.CLientId where c.Id = @Id and cd.Id = @OrderId ORDER BY c.Id, cd.Address;", con))
//            {
//                command.Parameters.AddWithValue("@Id", Id);
//                command.Parameters.AddWithValue("@OrderId", OrderID);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        cli = new ClientViewID()
//                        {
//                            Id = Convert.ToInt32(reader["Id"]),
//                            Name = Convert.ToString(reader["Name"]),
//                            Role = Convert.ToString(reader["Role"]),
//                            Email = Convert.ToString(reader["Email"]),
//                            Address = Convert.ToString(reader["Address"]),
//                            OrderId = Convert.ToInt32(reader["OrderId"])

//                        };

//                    }
//                }

//            }
//        }
//    }
//    catch (Exception e)
//    {
//        throw new Exception("Error getting all clients: " + e.Message);
//    }
//    return cli;
//}


//public bool UpdateAlll(ClientViewID ve)
//{
//    try
//    {
//        using (SqlConnection connection = new SqlConnection(_connectionString))
//        {
//            if (ve != null)
//            {

//                connection.Open();
//                using (SqlCommand command = new SqlCommand("Update Client set Name=@Name, Role=@Role, Email=@Email where Id = @Id ", connection))
//                {

//                    command.Parameters.AddWithValue("@Id", ve.Id);
//                    command.Parameters.AddWithValue("@Name", ve.Name);
//                    command.Parameters.AddWithValue("@Role", ve.Role);
//                    command.Parameters.AddWithValue("@Email", ve.Email);
//                    command.ExecuteNonQuery();
//                }



//                using (SqlCommand command = new SqlCommand("Update ClientDetails set Address = @Address where ClientId =@ClientId and Id = @OrderId", connection))
//                {

//                    command.Parameters.AddWithValue("@OrderId", ve.OrderId);
//                    command.Parameters.AddWithValue("@ClientId", ve.Id);
//                    command.Parameters.AddWithValue("@Address", ve.Address);
//                    int rowaff = (int)command.ExecuteNonQuery();

//                    return rowaff > 0;
//                }


//            }
//            else
//            {
//                return false;
//            }



//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error deleting all clients: " + ex.Message);
//    }


//}



//public bool DeleteFromList(int Id)
//{
//    int rowaff = 0;
//    using (SqlConnection con = new SqlConnection(_connectionString))
//    {
//        con.Open();
//        using (SqlCommand cmd = new SqlCommand("Delete ClientDetails where Id = @Id", con))
//        {
//            if (Id > 0)
//            {
//                cmd.Parameters.AddWithValue("@Id", Id);
//                rowaff = (int)cmd.ExecuteNonQuery();
//                return rowaff > 0;
//            }

//        }
//        return rowaff < 0;
//    }
//}



//public bool AddADDRL(ClientViewID li)
//{
//    int rowaff = 0;
//    using (SqlConnection con = new SqlConnection(_connectionString))
//    {
//        con.Open();
//        using (SqlCommand cmd = new SqlCommand("Insert into ClientDetails (ClientId,Address) values (@ClientId , @Address)", con))
//        {
//            cmd.Parameters.AddWithValue("@ClientId", li.Id);
//            cmd.Parameters.AddWithValue("@Address", li.Address);
//            rowaff = (int)cmd.ExecuteNonQuery();
//            return rowaff > 0;
//        }
//    }
//}


































