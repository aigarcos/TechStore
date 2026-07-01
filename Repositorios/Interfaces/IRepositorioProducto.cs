using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Repositorios.Interfaces
{
    public interface IRepositorioProducto
    {
        Producto BuscarPorCodigo(string codigo);
        List<Producto> ObtenerTodos();
        List<Producto> ObtenerConStockCritico(int umbral);
        // DescontarStock se hará en la transacción de venta, pero podríamos tener un método aislado si se necesita.
        void DescontarStock(int productoId, int cantidad);
        List<Producto> BuscarPorNombreOCodigo(string texto);
        void Registrar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int productoId);
    }
}
