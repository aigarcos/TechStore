using System.Collections.Generic;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Interfaces;

namespace TechStore.Servicios.Implementaciones
{
    public class ServicioProducto : IServicioProducto
    {
        private readonly IRepositorioProducto _repositorio;

        public ServicioProducto(IRepositorioProducto repositorio)
        {
            _repositorio = repositorio;
        }

        public Producto BuscarPorCodigo(string codigo)
        {
            return _repositorio.BuscarPorCodigo(codigo);
        }

        public List<Producto> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos();
        }

        public void DescontarStock(int productoId, int cantidad)
        {
            _repositorio.DescontarStock(productoId, cantidad);
        }

        public List<Producto> ObtenerConStockCritico(int umbral)
        {
            return _repositorio.ObtenerConStockCritico(umbral);
        }

        public List<Producto> BuscarPorNombreOCodigo(string texto)
        {
            return _repositorio.BuscarPorNombreOCodigo(texto);
        }

        public void Registrar(Producto producto)
        {
            _repositorio.Registrar(producto);
        }

        public void Actualizar(Producto producto)
        {
            _repositorio.Actualizar(producto);
        }

        public void Eliminar(int productoId)
        {
            _repositorio.Eliminar(productoId);
        }
    }
}
