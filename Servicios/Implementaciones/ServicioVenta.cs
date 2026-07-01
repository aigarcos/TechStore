using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Interfaces;
using TechStore.Infraestructura;

namespace TechStore.Servicios.Implementaciones
{
    public class ServicioVenta : IServicioVenta
    {
        private readonly IRepositorioVenta _repoVenta;
        private readonly ConexionBaseDatos _conexion;

        public ServicioVenta(IRepositorioVenta repoVenta, ConexionBaseDatos conexion)
        {
            _repoVenta = repoVenta;
            _conexion = conexion;
        }

        public int RegistrarVenta(Venta venta)
        {
            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar Venta
                        string queryVenta = "INSERT INTO Ventas (ClienteId, UsuarioId, Total) OUTPUT INSERTED.Id VALUES (@clienteId, @usuarioId, @total)";
                        int ventaId;
                        using (var cmd = new SqlCommand(queryVenta, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@clienteId", venta.ClienteId);
                            cmd.Parameters.AddWithValue("@usuarioId", venta.UsuarioId);
                            cmd.Parameters.AddWithValue("@total", venta.Total);
                            ventaId = (int)cmd.ExecuteScalar();
                        }

                        // 2. Insertar DetalleVenta y 3. Descontar stock
                        foreach (var detalle in venta.Detalles)
                        {
                            string queryDetalle = "INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES (@ventaId, @prodId, @cant, @precio, @subtotal)";
                            using (var cmd = new SqlCommand(queryDetalle, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@ventaId", ventaId);
                                cmd.Parameters.AddWithValue("@prodId", detalle.ProductoId);
                                cmd.Parameters.AddWithValue("@cant", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@precio", detalle.PrecioUnitario);
                                cmd.Parameters.AddWithValue("@subtotal", detalle.Subtotal);
                                cmd.ExecuteNonQuery();
                            }

                            string queryStock = "UPDATE Productos SET Stock = Stock - @cant WHERE Id = @prodId AND Stock >= @cant";
                            using (var cmd = new SqlCommand(queryStock, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@cant", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@prodId", detalle.ProductoId);
                                int rows = cmd.ExecuteNonQuery();
                                if (rows == 0) throw new Exception($"Stock insuficiente para el producto ID {detalle.ProductoId}.");
                            }
                        }

                        // 4. Generar registro en Facturas
                        string numFactura = $"FAC-{DateTime.Now.Year}-{ventaId:D6}";
                        string queryFactura = "INSERT INTO Facturas (VentaId, NumeroFactura, MontoTotal) VALUES (@ventaId, @numFactura, @montoTotal)";
                        using (var cmd = new SqlCommand(queryFactura, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@ventaId", ventaId);
                            cmd.Parameters.AddWithValue("@numFactura", numFactura);
                            cmd.Parameters.AddWithValue("@montoTotal", venta.Total);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return ventaId;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta)
        {
            return _repoVenta.ObtenerVentasPorFecha(desde, hasta);
        }

        public decimal ObtenerTotalDelDia()
        {
            return _repoVenta.ObtenerTotalDelDia();
        }

        public int ObtenerCantidadTransaccionesDelDia()
        {
            return _repoVenta.ObtenerCantidadTransaccionesDelDia();
        }

        public List<Venta> ObtenerUltimasVentas(int cantidad)
        {
            return _repoVenta.ObtenerUltimasVentas(cantidad);
        }

        public List<Venta> ObtenerVentasPorCliente(int clienteId)
        {
            return _repoVenta.ObtenerVentasPorCliente(clienteId);
        }
    }
}
