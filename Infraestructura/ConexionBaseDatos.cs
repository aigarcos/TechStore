using System.Data.SqlClient;

namespace TechStore.Infraestructura
{
    public class ConexionBaseDatos
    {
        private readonly string _connectionString;

        public ConexionBaseDatos()
        {
            _connectionString = @"Server=DESKTOP-B4FGGL9;Database=TechStore;Trusted_Connection=True;";
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
