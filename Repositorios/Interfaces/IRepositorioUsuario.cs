using TechStore.Modelos;

namespace TechStore.Repositorios.Interfaces
{
    public interface IRepositorioUsuario
    {
        Usuario ObtenerPorNombreUsuario(string nombreUsuario);
        void Actualizar(Usuario usuario);
    }
}
