using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TechStore.Formularios;
using TechStore.Infraestructura;

namespace TechStore
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicializar el contenedor DI
            var serviceProvider = ContenedorDI.Configurar();

            // Resolver el formulario de login principal
            var frmLogin = serviceProvider.GetRequiredService<FrmLogin>();

            Application.Run(frmLogin);
        }
    }
}
