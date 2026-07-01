using TechStore.Modelos;

namespace TechStore.Servicios.Interfaces
{
    public interface IServicioAutenticacion
    {
        Usuario Autenticar(string nombreUsuario, string contrasena);
    }
}
