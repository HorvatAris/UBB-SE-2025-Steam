using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Data
{
    public sealed partial class DataLink : IDataLink
    {
        // Constants for configuration keys and error messages
        private const string AppSettingsFileName = "appsettings.json";
        private const bool OptionalAppSettings = false;
        private const bool ReloadOnChange = true;

        private const string LocalDataSourceKey = "LocalDataSource";
        private const string InitialCatalogKey = "InitialCatalog";
        private const string UserIdKey = "UserId";
        private const string PasswordKey = "Password";

        private const string MissingConfigErrorMessage = "Database connection settings are missing in appsettings.json";
        private const string ConnectionInitErrorMessage = "Failed to initialize database connection.";
        private const string ConnectionFailedErrorMessage = "Failed to establish database connection. Please check your connection settings.";

        private const string ExecuteScalarErrorMessage = "Database error during ExecuteScalar operation: ";
        private const string ExecuteScalarCastErrorMessage = "Error during ExecuteScalar operation: ";
        private const string ExecuteScalarUnexpectedErrorMessage = "Unexpected error during ExecuteScalar operation: ";

        private const string ExecuteReaderErrorMessage = "Database error during ExecuteReader operation: ";
        private const string ExecuteReaderUnexpectedErrorMessage = "Error during ExecuteReader operation: ";

        private const string ExecuteNonQueryErrorMessage = "Database error during ExecuteNonQuery operation: ";
        private const string ExecuteNonQueryUnexpectedErrorMessage = "Error during ExecuteNonQuery operation: ";

        private const string ExecuteReaderAsyncErrorMessage = "Database error during ExecuteReaderAsync operation: ";
        private const string ExecuteReaderAsyncUnexpectedErrorMessage = "Error during ExecuteReaderAsync operation: ";

        private const string ExecuteNonQueryAsyncErrorMessage = "Database error during ExecuteNonQueryAsync operation: ";
        private const string ExecuteNonQueryAsyncUnexpectedErrorMessage = "Error during ExecuteNonQueryAsync operation: ";

        private const string ExecuteScalarSqlErrorMessage = "Database error during ExecuteScalarSql operation: ";
        private const string ExecuteScalarSqlCastErrorMessage = "Error during ExecuteScalarSql operation: ";
        private const string ExecuteScalarSqlUnexpectedErrorMessage = "Unexpected error during ExecuteScalarSql operation: ";

        private const string ExecuteReaderSqlErrorMessage = "Database error during ExecuteReaderSql operation: ";
        private const string ExecuteReaderSqlUnexpectedErrorMessage = "Error during ExecuteReaderSql operation: ";

        private const string ExecuteNonQuerySqlErrorMessage = "Database error during ExecuteNonQuerySql operation: ";
        private const string ExecuteNonQuerySqlUnexpectedErrorMessage = "Error during ExecuteNonQuerySql operation: ";

        private const string ExecuteReaderSqlAsyncErrorMessage = "Database error during ExecuteReaderSqlAsync operation: ";
        private const string ExecuteReaderSqlAsyncUnexpectedErrorMessage = "Error during ExecuteReaderSqlAsync operation: ";

        private const string ExecuteNonQuerySqlAsyncErrorMessage = "Database error during ExecuteNonQuerySqlAsync operation: ";
        private const string ExecuteNonQuerySqlAsyncUnexpectedErrorMessage = "Error during ExecuteNonQuerySqlAsync operation: ";

        private const bool UseIntegratedSecurity = true;
        private const bool TrustServerCertificate = true;

        private readonly string connectionString;
        private bool isConnectionDisposed;

        private DataLink()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(AppSettingsFileName, OptionalAppSettings, ReloadOnChange)
                    .Build();

                string? localDataSource = config[LocalDataSourceKey];
                string? initialCatalog = config[InitialCatalogKey];
                string? userId = config[UserIdKey];
                string? password = config[PasswordKey];

                if (string.IsNullOrWhiteSpace(localDataSource) || string.IsNullOrWhiteSpace(initialCatalog))
                {
                    throw new ConfigurationErrorsException(MissingConfigErrorMessage);
                }

                // Integrated Security connection string
                connectionString = $"Data Source={localDataSource};Initial Catalog={initialCatalog};Integrated Security={UseIntegratedSecurity};TrustServerCertificate={TrustServerCertificate};";

                Debug.WriteLine(connectionString);
                // Test the connection immediately
                using var testConnection = new SqlConnection(connectionString);
                testConnection.Open();
            }
            catch (SqlException exception)
            {
                throw new DatabaseConnectionException(ConnectionFailedErrorMessage, exception);
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException(ConnectionInitErrorMessage, exception);
            }
        }

        private SqlConnection CreateConnection()
        {
            if (isConnectionDisposed)
            {
                throw new ObjectDisposedException(nameof(DataLink));
            }

            return new SqlConnection(connectionString);
        }

        #region Stored Procedure Methods
        public T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                connection.Open();
                var result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return default;
                }

                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(result, Nullable.GetUnderlyingType(typeof(T))!);
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteScalarErrorMessage + exception.Message, exception);
            }
            catch (InvalidCastException exception)
            {
                throw new DatabaseOperationException(ExecuteScalarCastErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteScalarUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                using var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteReaderErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteReaderUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                return command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteNonQueryErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteNonQueryUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public async Task<DataTable> ExecuteReaderAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteReaderAsyncErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteReaderAsyncUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public async Task ExecuteNonQueryAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteNonQueryAsyncErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteNonQueryAsyncUnexpectedErrorMessage + exception.Message, exception);
            }
        }
        #endregion

        #region Direct SQL Methods
        public T? ExecuteScalarSql<T>(string sqlCommand, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = new SqlCommand(sqlCommand, connection)
                {
                    CommandType = CommandType.Text
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                connection.Open();
                var result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return default;
                }

                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(result, Nullable.GetUnderlyingType(typeof(T))!);
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteScalarSqlErrorMessage + exception.Message, exception);
            }
            catch (InvalidCastException exception)
            {
                throw new DatabaseOperationException(ExecuteScalarSqlCastErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteScalarSqlUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public DataTable ExecuteReaderSql(string sqlCommand, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(sqlCommand, connection)
                {
                    CommandType = CommandType.Text
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                using var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteReaderSqlErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteReaderSqlUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public int ExecuteNonQuerySql(string sqlCommand, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(sqlCommand, connection)
                {
                    CommandType = CommandType.Text
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                return command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteNonQuerySqlErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteNonQuerySqlUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public async Task<DataTable> ExecuteReaderSqlAsync(string sqlCommand, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(sqlCommand, connection)
                {
                    CommandType = CommandType.Text
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteReaderSqlAsyncErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteReaderSqlAsyncUnexpectedErrorMessage + exception.Message, exception);
            }
        }

        public async Task ExecuteNonQuerySqlAsync(string sqlCommand, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(sqlCommand, connection)
                {
                    CommandType = CommandType.Text
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                throw new DatabaseOperationException(ExecuteNonQuerySqlAsyncErrorMessage + exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException(ExecuteNonQuerySqlAsyncUnexpectedErrorMessage + exception.Message, exception);
            }
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposingStatus)
        {
            if (!isConnectionDisposed)
            {
                isConnectionDisposed = disposingStatus;
            }
        }
    }
}