using System;
using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Servicios.Interfaces
{
    public interface IServicioVenta
    {
        int RegistrarVenta(Venta venta);
        List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta);
        decimal ObtenerTotalDelDia();
        int ObtenerCantidadTransaccionesDelDia();
        List<Venta> ObtenerUltimasVentas(int cantidad);
        List<Venta> ObtenerVentasPorCliente(int clienteId);
    }
}
