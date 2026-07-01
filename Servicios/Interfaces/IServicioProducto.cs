using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Servicios.Interfaces
{
    public interface IServicioProducto
    {
        Producto BuscarPorCodigo(string codigo);
        List<Producto> ObtenerTodos();
        void DescontarStock(int productoId, int cantidad);
        List<Producto> ObtenerConStockCritico(int umbral);
        List<Producto> BuscarPorNombreOCodigo(string texto);
        void Registrar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int productoId);
    }
}
