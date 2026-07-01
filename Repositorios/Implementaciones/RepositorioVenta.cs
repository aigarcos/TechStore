using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Repositorios.Implementaciones
{
    public class RepositorioVenta : IRepositorioVenta
    {
        private readonly ConexionBaseDatos _conexion;

        public RepositorioVenta(ConexionBaseDatos conexion)
        {
            _conexion = conexion;
        }

        public List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta)
        {
            var ventas = new List<Venta>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Ventas WHERE Fecha >= @desde AND Fecha <= @hasta";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", desde.Date);
                    cmd.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1).AddTicks(-1));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                Id = (int)reader["Id"],
                                Fecha = (DateTime)reader["Fecha"],
                                ClienteId = (int)reader["ClienteId"],
                                UsuarioId = (int)reader["UsuarioId"],
                                Total = (decimal)reader["Total"]
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public decimal ObtenerTotalDelDia()
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT ISNULL(SUM(Total), 0) FROM Ventas WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }

        public int ObtenerCantidadTransaccionesDelDia()
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Ventas WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<Venta> ObtenerUltimasVentas(int cantidad)
        {
            var ventas = new List<Venta>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = $"SELECT TOP {cantidad} * FROM Ventas ORDER BY Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                Id = (int)reader["Id"],
                                Fecha = (DateTime)reader["Fecha"],
                                ClienteId = (int)reader["ClienteId"],
                                UsuarioId = (int)reader["UsuarioId"],
                                Total = (decimal)reader["Total"]
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public List<Venta> ObtenerVentasPorCliente(int clienteId)
        {
            var ventas = new List<Venta>();
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM Ventas WHERE ClienteId = @clienteId ORDER BY Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@clienteId", clienteId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                Id = (int)reader["Id"],
                                Fecha = (DateTime)reader["Fecha"],
                                ClienteId = (int)reader["ClienteId"],
                                UsuarioId = (int)reader["UsuarioId"],
                                Total = (decimal)reader["Total"]
                            });
                        }
                    }
                }
            }
            return ventas;
        }
    }
}
