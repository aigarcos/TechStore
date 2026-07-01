using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Repositorios.Implementaciones
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly ConexionBaseDatos _conexion;

        public RepositorioUsuario(ConexionBaseDatos conexion)
        {
            _conexion = conexion;
        }

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombreUsuario";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = (int)reader["Id"],
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                ContrasenaHash = reader["ContrasenaHash"].ToString(),
                                Rol = reader["Rol"].ToString(),
                                IntentosFailidos = (int)reader["IntentosFailidos"],
                                Activo = (bool)reader["Activo"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Actualizar(Usuario usuario)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE Usuarios SET ContrasenaHash = @pwd, Rol = @role, IntentosFailidos = @intentos, Activo = @activo WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pwd", usuario.ContrasenaHash);
                    cmd.Parameters.AddWithValue("@role", usuario.Rol);
                    cmd.Parameters.AddWithValue("@intentos", usuario.IntentosFailidos);
                    cmd.Parameters.AddWithValue("@activo", usuario.Activo);
                    cmd.Parameters.AddWithValue("@id", usuario.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
