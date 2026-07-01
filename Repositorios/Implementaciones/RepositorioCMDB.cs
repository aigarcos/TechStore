using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Repositorios.Implementaciones
{
    public class RepositorioCMDB : IRepositorioCMDB
    {
        private readonly ConexionBaseDatos _conexion;

        public RepositorioCMDB(ConexionBaseDatos conexion)
        {
            _conexion = conexion;
        }

        public void RegistrarItem(ItemConfiguracion item)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "INSERT INTO ItemsConfiguracion (Nombre, Tipo, Estado, NumeroSerie, Descripcion) VALUES (@nombre, @tipo, @estado, @serie, @desc)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                    cmd.Parameters.AddWithValue("@tipo", item.Tipo);
                    cmd.Parameters.AddWithValue("@estado", item.Estado);
                    cmd.Parameters.AddWithValue("@serie", item.NumeroSerie ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@desc", item.Descripcion ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarItem(ItemConfiguracion item)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE ItemsConfiguracion SET Nombre = @nombre, Tipo = @tipo, Estado = @estado, NumeroSerie = @serie, Descripcion = @desc WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                    cmd.Parameters.AddWithValue("@tipo", item.Tipo);
                    cmd.Parameters.AddWithValue("@estado", item.Estado);
                    cmd.Parameters.AddWithValue("@serie", item.NumeroSerie ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@desc", item.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CambiarEstado(int itemId, string nuevoEstado)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE ItemsConfiguracion SET Estado = @estado WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RelacionarItems(int origenId, int destinoId, string tipoRelacion)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (@origen, @destino, @tipo)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@origen", origenId);
                    cmd.Parameters.AddWithValue("@destino", destinoId);
                    cmd.Parameters.AddWithValue("@tipo", tipoRelacion);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<RelacionCI> ObtenerRelaciones(int itemId)
        {
            var relaciones = new List<RelacionCI>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM RelacionesCI WHERE CIOrigenId = @id OR CIDestinoId = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", itemId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            relaciones.Add(new RelacionCI
                            {
                                Id = (int)reader["Id"],
                                CIOrigenId = (int)reader["CIOrigenId"],
                                CIDestinoId = (int)reader["CIDestinoId"],
                                TipoRelacion = reader["TipoRelacion"].ToString(),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            });
                        }
                    }
                }
            }
            return relaciones;
        }

        public List<ItemConfiguracion> ObtenerTodos()
        {
            var items = new List<ItemConfiguracion>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM ItemsConfiguracion";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new ItemConfiguracion
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Tipo = reader["Tipo"].ToString(),
                                Estado = reader["Estado"].ToString(),
                                NumeroSerie = reader["NumeroSerie"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                FechaAlta = (DateTime)reader["FechaAlta"],
                                InicioMantenimiento = reader["InicioMantenimiento"] as DateTime?,
                                FinMantenimiento = reader["FinMantenimiento"] as DateTime?,
                                MotivoMantenimiento = reader["MotivoMantenimiento"].ToString(),
                                FechaBaja = reader["FechaBaja"] as DateTime?,
                                MotivoBaja = reader["MotivoBaja"].ToString()
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<ItemConfiguracion> ObtenerPorTipo(string tipo)
        {
            var items = new List<ItemConfiguracion>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM ItemsConfiguracion WHERE Tipo = @tipo";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new ItemConfiguracion
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Tipo = reader["Tipo"].ToString(),
                                Estado = reader["Estado"].ToString(),
                                NumeroSerie = reader["NumeroSerie"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                FechaAlta = (DateTime)reader["FechaAlta"],
                                InicioMantenimiento = reader["InicioMantenimiento"] as DateTime?,
                                FinMantenimiento = reader["FinMantenimiento"] as DateTime?,
                                MotivoMantenimiento = reader["MotivoMantenimiento"].ToString(),
                                FechaBaja = reader["FechaBaja"] as DateTime?,
                                MotivoBaja = reader["MotivoBaja"].ToString()
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<ItemConfiguracion> ObtenerPorEstado(string estado)
        {
            var items = new List<ItemConfiguracion>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM ItemsConfiguracion WHERE Estado = @estado";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@estado", estado);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new ItemConfiguracion
                            {
                                Id = (int)reader["Id"],
                                Nombre = reader["Nombre"].ToString(),
                                Tipo = reader["Tipo"].ToString(),
                                Estado = reader["Estado"].ToString(),
                                NumeroSerie = reader["NumeroSerie"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                FechaAlta = (DateTime)reader["FechaAlta"],
                                InicioMantenimiento = reader["InicioMantenimiento"] as DateTime?,
                                FinMantenimiento = reader["FinMantenimiento"] as DateTime?,
                                MotivoMantenimiento = reader["MotivoMantenimiento"].ToString(),
                                FechaBaja = reader["FechaBaja"] as DateTime?,
                                MotivoBaja = reader["MotivoBaja"].ToString()
                            });
                        }
                    }
                }
            }
            return items;
        }

        public int ObtenerConteoActivos()
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM ItemsConfiguracion WHERE Estado = 'Activo'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public int ObtenerConteoEnMantenimiento()
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM ItemsConfiguracion WHERE Estado = 'EnMantenimiento'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void IniciarMantenimiento(int itemId, string motivo, DateTime inicio)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE ItemsConfiguracion SET Estado = 'EnMantenimiento', InicioMantenimiento = @inicio, MotivoMantenimiento = @motivo WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", inicio);
                    cmd.Parameters.AddWithValue("@motivo", motivo);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void FinalizarMantenimiento(int itemId, DateTime fin)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE ItemsConfiguracion SET Estado = 'Activo', FinMantenimiento = @fin WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fin", fin);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DarDeBaja(int itemId, string motivo, DateTime fechaBaja)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "UPDATE ItemsConfiguracion SET Estado = 'Baja', FechaBaja = @fechaBaja, MotivoBaja = @motivo WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fechaBaja", fechaBaja);
                    cmd.Parameters.AddWithValue("@motivo", motivo);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
