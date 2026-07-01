using System;
using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Repositorios.Interfaces
{
    public interface IRepositorioVenta
    {

        
        List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta);
        decimal ObtenerTotalDelDia();
        int ObtenerCantidadTransaccionesDelDia();
        List<Venta> ObtenerUltimasVentas(int cantidad);
        List<Venta> ObtenerVentasPorCliente(int clienteId);
    }
}
