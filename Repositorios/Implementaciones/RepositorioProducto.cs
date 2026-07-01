using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Repositorios.Implementaciones
{
    public class RepositorioProducto : IRepositorioProducto
    {
        private readonly ConexionBaseDatos _conexion;

        public RepositorioProducto(ConexionBaseDatos conexion)
        {
            _conexion = conexion;
        }

        public Producto BuscarPorCodigo(string codigo)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Productos WHERE Codigo = @codigo";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                Id = (int)reader["Id"],
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = (decimal)reader["Precio"],
                                Stock = (int)reader["Stock"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Productos";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                Id = (int)reader["Id"],
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = (decimal)reader["Precio"],
                                Stock = (int)reader["Stock"]
                            });
                        }
                    }
                }
            }
            return productos;
        }

        public List<Producto> ObtenerConStockCritico(int umbral)
        {
            var productos = new List<Producto>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Productos WHERE Stock <= @umbral";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@umbral", umbral);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                Id = (int)reader["Id"],
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = (decimal)reader["Precio"],
                                Stock = (int)reader["Stock"]
                            });
                        }
                    }
                }
            }
            return productos;
        }

        public void DescontarStock(int productoId, int cantidad)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE Productos SET Stock = Stock - @cantidad WHERE Id = @id AND Stock >= @cantidad";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@id", productoId);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0) throw new Exception("Error al descontar stock o stock insuficiente.");
                }
            }
        }

        public List<Producto> BuscarPorNombreOCodigo(string texto)
        {
            var productos = new List<Producto>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Productos WHERE Nombre LIKE @texto OR Codigo LIKE @texto";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                Id = (int)reader["Id"],
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precio = (decimal)reader["Precio"],
                                Stock = (int)reader["Stock"]
                            });
                        }
                    }
                }
            }
            return productos;
        }

        public void Registrar(Producto producto)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "INSERT INTO Productos (Codigo, Nombre, Categoria, Precio, Stock) VALUES (@codigo, @nombre, @categoria, @precio, @stock)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", producto.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@categoria", producto.Categoria);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@stock", producto.Stock);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Producto producto)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE Productos SET Codigo = @codigo, Nombre = @nombre, Categoria = @categoria, Precio = @precio, Stock = @stock WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", producto.Id);
                    cmd.Parameters.AddWithValue("@codigo", producto.Codigo);
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@categoria", producto.Categoria);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@stock", producto.Stock);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int productoId)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "DELETE FROM Productos WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
