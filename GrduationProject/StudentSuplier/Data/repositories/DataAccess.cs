using StudentSuplier.Data.Repositories;
using System.Data;
using System.Data;
using System.Data.SqlClient;

namespace StudentSuplier.Data.repositories
{
    public class DataAccess : IDataAccess
    {

        private readonly string _connectionString;

        public DataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<DataSet> ExecuteMultipleQueryAsync(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    await connection.OpenAsync();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    await connection.OpenAsync();
                    return await command.ExecuteScalarAsync();
                }
            }
        }
        /////////////////////////////////////////////////////////////////////
        ///


        public DataTable ExecuteQuery(string query, SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        // تنفيذ الدالة المزامنة ExecuteNonQuery
        public int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        // تنفيذ الدالة غير المتزامنة ExecuteQueryAsync
        public async Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        // تنفيذ الدالة غير المتزامنة ExecuteNonQueryAsync
        public async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }

        // تنفيذ الدالة غير المتزامنة ExecuteScalarAsync
        
    }
}

