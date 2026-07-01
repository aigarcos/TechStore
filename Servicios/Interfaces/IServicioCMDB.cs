using System.Collections.Generic;
using TechStore.Modelos;

namespace TechStore.Servicios.Interfaces
{
    public interface IServicioCMDB
    {
        void RegistrarItem(ItemConfiguracion item);
        void ActualizarItem(ItemConfiguracion item);
        void CambiarEstado(int itemId, string nuevoEstado);
        void RelacionarItems(int origenId, int destinoId, string tipoRelacion);
        List<RelacionCI> ObtenerRelaciones(int itemId);
        List<ItemConfiguracion> ObtenerTodos();
        List<ItemConfiguracion> ObtenerPorTipo(string tipo);
        List<ItemConfiguracion> ObtenerPorEstado(string estado);
        int ObtenerConteoActivos();
        int ObtenerConteoEnMantenimiento();
        void IniciarMantenimiento(int itemId, string motivo, System.DateTime inicio);
        void FinalizarMantenimiento(int itemId, System.DateTime fin);
        void DarDeBaja(int itemId, string motivo, System.DateTime fechaBaja);
    }
}
