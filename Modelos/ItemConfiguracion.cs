using System;

namespace TechStore.Modelos
{
    public class ItemConfiguracion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public string NumeroSerie { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? InicioMantenimiento { get; set; }
        public DateTime? FinMantenimiento { get; set; }
        public string MotivoMantenimiento { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string MotivoBaja { get; set; }
    }
}
