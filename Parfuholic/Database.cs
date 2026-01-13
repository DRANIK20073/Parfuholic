using System.Data.SqlClient;

namespace Parfuholic
{
    public static class Database
    {
        public static string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=ParfuholicDB;Trusted_Connection=True;";
    }
}
