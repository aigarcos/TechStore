using System;
using System.Security.Cryptography;
using System.Text;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Interfaces;

namespace TechStore.Servicios.Implementaciones
{
    public class SesionActual
    {
        public static Usuario Usuario { get; set; }
    }

    public class ServicioAutenticacion : IServicioAutenticacion
    {
        private readonly IRepositorioUsuario _repositorio;

        public ServicioAutenticacion(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        public Usuario Autenticar(string nombreUsuario, string contrasena)
        {
            var usuario = _repositorio.ObtenerPorNombreUsuario(nombreUsuario);
            if (usuario == null) return null;

            string hashIngresado = CalcularSHA256(contrasena);

            if (usuario.ContrasenaHash == hashIngresado)
            {
                usuario.IntentosFailidos = 0;
                _repositorio.Actualizar(usuario);
                SesionActual.Usuario = usuario;
                return usuario;
            }
            else
            {
                usuario.IntentosFailidos++;
                _repositorio.Actualizar(usuario);
                return null;
            }
        }

        private string CalcularSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
