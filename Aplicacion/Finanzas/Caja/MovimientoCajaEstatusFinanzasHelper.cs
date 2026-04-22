using System;
using System.Collections.Generic;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Cobranza.Liquidaciones;
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

            if (m.LiquidacionCobranzaId.HasValue && estatusLiquidacionPorId != null &&
                estatusLiquidacionPorId.TryGetValue(m.LiquidacionCobranzaId.Value, out var est))
            {
                if (EstatusLiquidacionCobranzaExtensions.EqualsStored(est, EstatusLiquidacionCobranza.Confirmada))
                {
                    return "COBRADO";
                }

                return "ENVIADA";
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

            if (!m.LiquidacionCobranzaId.HasValue)
            {
                return false;
            }

            return estatusLiquidacionPorId.TryGetValue(m.LiquidacionCobranzaId.Value, out var est) &&
                   EstatusLiquidacionCobranzaExtensions.EqualsStored(est, EstatusLiquidacionCobranza.Confirmada);
        }
    }
}
