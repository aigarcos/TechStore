using Microsoft.Extensions.DependencyInjection;
using TechStore.Formularios;
using TechStore.Repositorios.Implementaciones;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Implementaciones;
using TechStore.Servicios.Interfaces;

namespace TechStore.Infraestructura
{
    public static class ContenedorDI
    {
        public static ServiceProvider Configurar()
        {
            var services = new ServiceCollection();

            // Infraestructura
            services.AddSingleton<ConexionBaseDatos>();

            // Repositorios
            services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
            services.AddTransient<IRepositorioVenta, RepositorioVenta>();
            services.AddTransient<IRepositorioProducto, RepositorioProducto>();
            services.AddTransient<IRepositorioCliente, RepositorioCliente>();
            services.AddTransient<IRepositorioCMDB, RepositorioCMDB>();

            // Servicios
            services.AddTransient<IServicioAutenticacion, ServicioAutenticacion>();
            services.AddTransient<IServicioVenta, ServicioVenta>();
            services.AddTransient<IServicioProducto, ServicioProducto>();
            services.AddTransient<IServicioCliente, ServicioCliente>();
            services.AddTransient<IServicioCMDB, ServicioCMDB>();

            // Formularios
            services.AddTransient<FrmLogin>();
            services.AddTransient<FrmPrincipal>();
            services.AddTransient<FrmVenta>();
            services.AddTransient<FrmInventario>();
            services.AddTransient<FrmClientes>();
            services.AddTransient<FrmReportes>();
            services.AddTransient<FrmCMDB>();

            return services.BuildServiceProvider();
        }
    }
}
