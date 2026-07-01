using System.Collections.Generic;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Interfaces;

namespace TechStore.Servicios.Implementaciones
{
    public class ServicioCliente : IServicioCliente
    {
        private readonly IRepositorioCliente _repositorio;

        public ServicioCliente(IRepositorioCliente repositorio)
        {
            _repositorio = repositorio;
        }

        public Cliente BuscarPorDocumento(string documento)
        {
            return _repositorio.BuscarPorDocumento(documento);
        }

        public void Registrar(Cliente cliente)
        {
            _repositorio.Registrar(cliente);
        }

        public void Actualizar(Cliente cliente)
        {
            _repositorio.Actualizar(cliente);
        }

        public List<Cliente> BuscarPorNombreODocumento(string texto)
        {
            return _repositorio.BuscarPorNombreODocumento(texto);
        }

        public void Eliminar(int clienteId)
        {
            _repositorio.Eliminar(clienteId);
        }
    }
}
