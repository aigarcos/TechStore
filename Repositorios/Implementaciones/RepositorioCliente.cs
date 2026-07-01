using System.Collections.Generic;
using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Repositorios.Implementaciones
{
    public class RepositorioCliente : IRepositorioCliente
    {
        private readonly ConexionBaseDatos _conexion;

        public RepositorioCliente(ConexionBaseDatos conexion)
        {
            _conexion = conexion;
        }

        public Cliente BuscarPorDocumento(string documento)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Clientes WHERE Documento = @doc";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@doc", documento);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Cliente
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Documento = reader["Documento"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Correo = reader["Correo"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Registrar(Cliente cliente)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "INSERT INTO Clientes (Nombre, Documento, Telefono, Correo) VALUES (@nombre, @doc, @tel, @correo)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@doc", cliente.Documento);
                    cmd.Parameters.AddWithValue("@tel", cliente.Telefono ?? (object)System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@correo", cliente.Correo ?? (object)System.DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Cliente cliente)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE Clientes SET Nombre = @nombre, Telefono = @tel, Correo = @correo WHERE Documento = @doc";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@tel", cliente.Telefono ?? (object)System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@correo", cliente.Correo ?? (object)System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@doc", cliente.Documento);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Cliente> BuscarPorNombreODocumento(string texto)
        {
            var clientes = new List<Cliente>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Clientes WHERE Nombre LIKE @texto OR Documento LIKE @texto";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientes.Add(new Cliente
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Documento = reader["Documento"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Correo = reader["Correo"].ToString()
                            });
                        }
                    }
                }
            }
            return clientes;
        }

        public void Eliminar(int clienteId)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "DELETE FROM Clientes WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", clienteId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
