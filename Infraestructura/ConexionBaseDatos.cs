using System.Data.SqlClient;

namespace TechStore.Infraestructura
{
    public class ConexionBaseDatos
    {
        private readonly string _connectionString;

        public ConexionBaseDatos()
        {
            _connectionString = @"Server=DESKTOP-G177510;Database=TechStore;Trusted_Connection=True;";
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
