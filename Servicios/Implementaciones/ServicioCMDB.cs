using System.Collections.Generic;
using TechStore.Modelos;
using TechStore.Repositorios.Interfaces;
using TechStore.Servicios.Interfaces;

namespace TechStore.Servicios.Implementaciones
{
    public class ServicioCMDB : IServicioCMDB
    {
        private readonly IRepositorioCMDB _repositorio;

        public ServicioCMDB(IRepositorioCMDB repositorio)
        {
            _repositorio = repositorio;
        }

        public void RegistrarItem(ItemConfiguracion item)
        {
            _repositorio.RegistrarItem(item);
        }

        public void ActualizarItem(ItemConfiguracion item)
        {
            _repositorio.ActualizarItem(item);
        }

        public void CambiarEstado(int itemId, string nuevoEstado)
        {
            _repositorio.CambiarEstado(itemId, nuevoEstado);
        }

        public void RelacionarItems(int origenId, int destinoId, string tipoRelacion)
        {
            _repositorio.RelacionarItems(origenId, destinoId, tipoRelacion);
        }

        public List<RelacionCI> ObtenerRelaciones(int itemId)
        {
            return _repositorio.ObtenerRelaciones(itemId);
        }

        public List<ItemConfiguracion> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos();
        }

        public List<ItemConfiguracion> ObtenerPorTipo(string tipo)
        {
            return _repositorio.ObtenerPorTipo(tipo);
        }

        public List<ItemConfiguracion> ObtenerPorEstado(string estado)
        {
            return _repositorio.ObtenerPorEstado(estado);
        }

        public int ObtenerConteoActivos()
        {
            return _repositorio.ObtenerConteoActivos();
        }

        public int ObtenerConteoEnMantenimiento()
        {
            return _repositorio.ObtenerConteoEnMantenimiento();
        }

        public void IniciarMantenimiento(int itemId, string motivo, System.DateTime inicio)
        {
            _repositorio.IniciarMantenimiento(itemId, motivo, inicio);
        }

        public void FinalizarMantenimiento(int itemId, System.DateTime fin)
        {
            _repositorio.FinalizarMantenimiento(itemId, fin);
        }

        public void DarDeBaja(int itemId, string motivo, System.DateTime fechaBaja)
        {
            _repositorio.DarDeBaja(itemId, motivo, fechaBaja);
        }
    }
}
