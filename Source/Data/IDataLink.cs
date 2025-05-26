using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Data
{
    public interface IDataLink : IDisposable
    {
        // Methods for running Stored Procedures
        T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null);
        DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null);
        int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null);
        Task<DataTable> ExecuteReaderAsync(string storedProcedure, SqlParameter[]? sqlParameters = null);
        Task ExecuteNonQueryAsync(string storedProcedure, SqlParameter[]? sqlParameters = null);

        // Methods for running direct SQL Commands
        T? ExecuteScalarSql<T>(string sqlCommand, SqlParameter[]? sqlParameters = null);
        DataTable ExecuteReaderSql(string sqlCommand, SqlParameter[]? sqlParameters = null);
        int ExecuteNonQuerySql(string sqlCommand, SqlParameter[]? sqlParameters = null);
        Task<DataTable> ExecuteReaderSqlAsync(string sqlCommand, SqlParameter[]? sqlParameters = null);
        Task ExecuteNonQuerySqlAsync(string sqlCommand, SqlParameter[]? sqlParameters = null);
    }
}