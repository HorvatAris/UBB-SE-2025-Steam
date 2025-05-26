using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Data
{
    public interface IDatabaseConnection
    {
        public string GetConnectionString();

        public SqlConnection GetConnection();

        public void Connect();

        public void Disconnect();

        public DataSet ExecuteQuery(string query, string entityName);

        public void ExecuteInsert(string tableName, Dictionary<string, object> parameters);

        public void ExecuteDelete(string tableName, string columnName, object value);

        public void ExecuteUpdate(string tableName, string columnToUpdate, string columnToMatch, object updateValue, object matchValue);

        public void ExecuteNonQuery(string query);

        public void ExecuteDeleteWithAnd(string tableName, Dictionary<string, object> parameters);
    }
}
