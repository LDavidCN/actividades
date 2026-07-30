using System;
using System.Data;
using System.Data.SqlClient;

namespace Actividad3CRUD
{
    internal static class Database
    {
        internal const string ConnectionString = @"Data Source=DAVID\SQLEXPRESS;Initial Catalog=Actividad3CRUD;Integrated Security=True;Connect Timeout=5";

        internal static DataTable Query(string sql)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var adapter = new SqlDataAdapter(sql, connection))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        internal static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddRange(parameters);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }
}
