
using System.Data;
using EmployeeeApp.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeeApp.Data
{
    public class EmployeeData
    {
        private readonly string _connectionString;

        public static IConfiguration Configuration { get; set; }

        public EmployeeData()
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

        public List<Employee> GetAll(int page = 1, int pageSize = 10)
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT * FROM EmployeeDetails ORDER BY Id ASC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection)) 
                    {
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FirstName = Convert.ToString(reader["FirstName"]),
                                    LastName = Convert.ToString(reader["LastName"]),
                                    Department = Convert.ToString(reader["Department"]),
                                    Email = Convert.ToString(reader["Email"]),
                                    Dateofjoining = reader["Dateofjoining"] != DBNull.Value
                                            ? Convert.ToDateTime(reader["Dateofjoining"])
                                            : default,
                                    Profilepic = reader["Profilepic"] != DBNull.Value ? Convert.ToString(reader["Profilepic"]) : null
                                }); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Retrieving Employee Data: {ex.Message}");
            }

            return employees;
        }

        public int GetTotalCount()
        {
            int count = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM EmployeeDetails", connection)) 
                    {
                        count = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Retrieving Employee Count: {ex.Message}");
            }

            return count;
        }


        public bool Insert(Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                     "INSERT INTO EmployeeDetails (FirstName, LastName, Department, Email, Dateofjoining, Profilepic) VALUES (@FirstName, @LastName, @Department, @Email, @Dateofjoining, @Profilepic)", connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Dateofjoining", employee.Dateofjoining);
                        command.Parameters.AddWithValue("@Profilepic", string.IsNullOrEmpty(employee.Profilepic) ? (object)DBNull.Value : employee.Profilepic);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Insert Employee: {ex.Message}");
                return false;
            }


        }

        public Employee GetById(int id)
        {
            Employee employee = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT * FROM EmployeeDetails WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employee = new Employee
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FirstName = Convert.ToString(reader["FirstName"]),
                                    LastName = Convert.ToString(reader["LastName"]),
                                    Department = Convert.ToString(reader["Department"]),
                                    Email = Convert.ToString(reader["Email"]),
                                    Dateofjoining = reader["Dateofjoining"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["Dateofjoining"])
                                        : default,
                                    Profilepic = reader["Profilepic"] != DBNull.Value ? Convert.ToString(reader["Profilepic"]) : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Retrieving Employee by ID: {ex.Message}");
            }

            return employee;
        }

        public bool Update(Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "UPDATE EmployeeDetails SET FirstName = @FirstName, LastName = @LastName, Department = @Department, Email = @Email, Dateofjoining = @Dateofjoining, Profilepic = @Profilepic WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", employee.Id);
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Dateofjoining", employee.Dateofjoining);
                        command.Parameters.AddWithValue("@Profilepic", string.IsNullOrEmpty(employee.Profilepic) ? (object)DBNull.Value : employee.Profilepic);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Updating Employee: {ex.Message}");
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM EmployeeDetails WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Deleting Employee: {ex.Message}");
                return false;
            }
        }

    }
}