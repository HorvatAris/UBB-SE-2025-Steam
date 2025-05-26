using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using BusinessLayer.Data;

namespace BusinessLayer.Data
{
    public class DatabaseConnection : IDatabaseConnection
    {
        // const string CONNECTION_STRING = "Server=GHASTERLAPTOP;Database=Community;User Id=sa;Password=1808;TrustServerCertificate=True";
        public string ConnectionString;
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }
        public SqlConnection Connection;
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public DatabaseConnection()
        {
            string connectionString = DatabaseConnectionSettings.CONNECTION_STRING;
            ConnectionString = connectionString;
            Connection = new SqlConnection(connectionString);
        }

        public void Connect()
        {
            Connection.Open();
        }

        public void Disconnect()
        {
            Connection.Close();
        }

        public DataSet ExecuteQuery(string query, string entityName)
        {
            DataSet dataSet = new DataSet();

            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, Connection);

            dataAdapter.Fill(dataSet, entityName);

            return dataSet;
        }

        public void ExecuteInsert(string tableName, Dictionary<string, object> parameters)
        {
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (var param in parameters)
            {
                columns.Append(param.Key + ", ");
                values.Append("@" + param.Key + ", ");
            }

            columns.Length -= 2;
            values.Length -= 2;

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            SqlCommand command = new SqlCommand(query, Connection);

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.Key, param.Value);
            }

            command.ExecuteNonQuery();
        }

        public void ExecuteDelete(string tableName, string columnName, object value)
        {
            string query = $"DELETE FROM {tableName} WHERE {columnName} = @Value";

            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@value", value);

            command.ExecuteNonQuery();
        }

        public void ExecuteDeleteWithAnd(string tableName, Dictionary<string, object> parameters)
        {
            StringBuilder query = new StringBuilder();

            if (parameters.Count == 0)
            {
                return;
            }

            query.Append($"DELETE FROM {tableName} WHERE");

            foreach (var param in parameters)
            {
                query.Append($" {param.Key} = @{param.Key} AND");
            }

            int numberOfCharactersToBeRemoved = 3;
            query.Remove(query.Length - numberOfCharactersToBeRemoved, numberOfCharactersToBeRemoved);

            SqlCommand command = new SqlCommand(query.ToString(), Connection);

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.Key, param.Value);
            }

            command.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string query)
        {
            SqlCommand command = new SqlCommand(query, Connection);
            command.ExecuteNonQuery();
        }

        public void ExecuteUpdate(string tableName, string columnToUpdate, string columnToMatch, object updateValue, object matchValue)
        {
            string query = $"UPDATE {tableName} SET {columnToUpdate} = @columnToUpdate WHERE {columnToMatch} = @matchValue";

            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@columnToUpdate", updateValue);
            command.Parameters.AddWithValue("@matchValue", matchValue);

            command.ExecuteNonQuery();
        }

        public async Task ExecuteNonQueryAsync(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using var connection = CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> ExecuteReaderAsync(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using var connection = CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            var dataTable = new DataTable();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}
