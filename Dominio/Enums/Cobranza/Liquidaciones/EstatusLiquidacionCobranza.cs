using System;

namespace FinancieraSoluciones.Domain.Enums.Cobranza.Liquidaciones
{
    public enum EstatusLiquidacionCobranza
    {
        Enviada,
        Confirmada,
        Rechazada
    }

    public static class EstatusLiquidacionCobranzaExtensions
    {
        public static string ToStoredString(this EstatusLiquidacionCobranza estatus) => estatus.ToString();

        public static bool EqualsStored(string? stored, EstatusLiquidacionCobranza expected) =>
            string.Equals(stored?.Trim(), expected.ToStoredString(), StringComparison.Ordinal);
    }
}
