using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentSuplier.Data.Repositories;
using StudentSuplier.Models;

namespace StudentSuplier.Data
{
    // Data/EmployeeDataAccess.cs


    public class EmployeeDataAccess 
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EmployeeDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }



        public async Task<int> AddEmployee(Models.User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("ADD_Users_PRC", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.Parameters.AddWithValue("@usertype", user.usertype);

                    await connection.OpenAsync();

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return 0;
            }
        }

        public async Task<User> GetEmployeeById(string email, string password)
        {
            User user = null;

            //using (SqlConnection connection = new SqlConnection(_connectionString))
            //{
            //    SqlCommand command = new SqlCommand("login", connection);
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.Parameters.AddWithValue("@email", email);
            //    command.Parameters.AddWithValue("@password", password);
            //    await connection.OpenAsync();

            //    using (SqlDataReader reader = await command.ExecuteReaderAsync())
            //    {
            //        if (await reader.ReadAsync())
            //        {
            //            user = new User
            //            {
            //                UserId  = Convert.ToInt32(reader["user_Id"]),
            //                Username  = reader["username"].ToString(),
            //                Email = reader["email"].ToString(),
            //                Password = reader["password"].ToString(),
            //                usertype  = reader["usertype"].ToString(),
            //            };
            //        }
            //    }
            //}
            SqlConnection connection = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Users WHERE email=N'" + email + "' and password =N'" + password + "'";
            SqlCommand cmd = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    UserId = Convert.ToInt32(reader["user_Id"]),
                    Username = reader["username"].ToString(),
                    Email = reader["email"].ToString(),
                    Password = reader["password"].ToString(),
                    usertype = reader["usertype"].ToString(),
                };
            }

            return user;
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DEL_Users_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@user_Id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }



        public async Task<int> AddLibraryAsync(Library library)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ADD_Libraries_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LibraryName", library.LibraryName);
                    command.Parameters.AddWithValue("@Location", (object)library.Location ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)library.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@imgurl", (object)library.ImageUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@WorkingHours", (object)library.WorkingHour ?? DBNull.Value);
                    

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<Library> GetLibraryByIdAsync(string LibraryName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GET_Libraries_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LibraryName", LibraryName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Library
                            {
                                LibraryId = reader.GetInt32(0),
                                LibraryName = reader.GetString(1),
                                Location = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkingHour = reader.IsDBNull(5) ? null : reader.GetString(5),
                                
                            };
                        }
                    }
                }
            }

            return null;
        }


        public async Task<int> UpdateLibraryAsync(Library library)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("UPD_Libraries_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LibraryId", library.LibraryId);
                    command.Parameters.AddWithValue("@LibraryName", library.LibraryName);
                    command.Parameters.AddWithValue("@Location", (object)library.Location ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)library.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@imgurl", (object)library.ImageUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@WorkingHours", (object)library.WorkingHour ?? DBNull.Value);
                    

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> DeleteLibraryAsync(int id, string LibraryName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DEL_Libraries_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@LibraryId", id);
                    command.Parameters.AddWithValue("@LibraryName", LibraryName);


                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<List<Library>> SearchLibrariesAsync(string searchText)
        {
            var libraries = new List<Library>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SEARCH_Libraries_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchText", searchText);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            libraries.Add(new Library
                            {
                                LibraryId = reader.GetInt32(0),
                                LibraryName = reader.GetString(1),
                                Location = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkingHour = reader.IsDBNull(5) ? null : reader.GetString(5),
                                
                            });
                        }
                    }
                }
            }

            return libraries;
        }

        public async Task<Product> GetItemByIdAsync(string Name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GET_Items_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", Name);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Product
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Category = reader.IsDBNull(5) ? null : reader.GetString(5),
                                
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> AddItemAsync(Product item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ADD_Items_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", item.ProductName);
                    command.Parameters.AddWithValue("@description", (object)item.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price", (object)item.Price ?? DBNull.Value);
                    command.Parameters.AddWithValue("@imgurl", (object)item.ImageUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@category", (object)item.Category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LibraryID", (object)item.LibraryId ?? DBNull.Value);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateItemAsync(Product item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("UPD_Items_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@item_Id", item.ProductId);
                    command.Parameters.AddWithValue("@Name", item.ProductName);
                    command.Parameters.AddWithValue("@description", (object)item.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price", (object)item.Price ?? DBNull.Value);
                    command.Parameters.AddWithValue("@imgurl", (object)item.ImageUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@category", (object)item.Category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LibraryID", (object)item.LibraryId ?? DBNull.Value);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteItemAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DEL_Items_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@item_Id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Product>> SearchItemsAsync(string searchText)
        {
            var items = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SEARCH_Items_PRC", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchText", searchText);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new Product
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Category = reader.IsDBNull(5) ? null : reader.GetString(5),
                                
                            });
                        }
                    }
                }
            }

            return items;
        }


        ////////////////////////////////////////////////////////////////////////////
        

        ////////////////////////////////////////////////////////////////////////////
        


    }
}



