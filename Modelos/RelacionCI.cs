using System;

namespace TechStore.Modelos
{
    public class RelacionCI
    {
        public int Id { get; set; }
        public int CIOrigenId { get; set; }
        public int CIDestinoId { get; set; }
        public string TipoRelacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
