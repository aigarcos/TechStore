using System;

namespace TechStore.Modelos
{
    public class Factura
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
