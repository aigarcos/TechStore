using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Repositorios.Interfaces
{
    public interface IRepositorioCliente
    {
        Cliente BuscarPorDocumento(string documento);
        void Registrar(Cliente cliente);
        void Actualizar(Cliente cliente);
        List<Cliente> BuscarPorNombreODocumento(string texto);
        void Eliminar(int clienteId);
    }
}
