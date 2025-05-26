using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Data
{
    public class DatabaseConnectionSettings
    {
        public static string CONNECTION_STRING = "Integrated Security=True;TrustServerCertificate=True;data source=.;initial catalog=Community;user id=sa";
    }
}
