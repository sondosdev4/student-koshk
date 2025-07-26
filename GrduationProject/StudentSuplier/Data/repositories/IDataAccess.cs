using System.Data;
using System.Data.SqlClient;

namespace StudentSuplier.Data.Repositories
    {
        public interface IDataAccess
        {
       // Task<DataSet> ExecuteQuery(string query, SqlParameter[] parameters);
       DataTable ExecuteQuery(string query, SqlParameter[] parameters);
            int ExecuteNonQuery(string query, SqlParameter[] parameters);

            Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[] parameters);
            Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string query, SqlParameter[] parameters);

    }
}

