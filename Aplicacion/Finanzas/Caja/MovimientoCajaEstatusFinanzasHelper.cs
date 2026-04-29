using System;
using System.Collections.Generic;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;

namespace FinancieraSoluciones.Application.Finanzas.Caja
{
    public static class MovimientoCajaEstatusFinanzasHelper
    {
        public static string? CalcularEstatusFichaFinanzas(
            MovimientoCaja m,
            IReadOnlyDictionary<Guid, string>? estatusLiquidacionPorId)
        {
            if (!m.CobradorId.HasValue)
            {
                return null;
            }

            if (!TipoMovimientoCajaExtensions.EqualsStored(m.Tipo, TipoMovimientoCaja.Ficha) &&
                !TipoMovimientoCajaExtensions.EqualsStored(m.Tipo, TipoMovimientoCaja.Ingreso))
            {
                return null;
            }

            if (m.CorteCajaId.HasValue)
            {
                return "EN_CORTE";
            }

            if (m.RecibidoCaja)
            {
                return "COBRADO";
            }

            return "PENDIENTE";
        }

        public static bool EsCobradoParaResumen(
            MovimientoCaja m,
            IReadOnlyDictionary<Guid, string> estatusLiquidacionPorId)
        {
            if (m.RecibidoCaja)
            {
                return true;
            }

            return false;
        }
    }
}
