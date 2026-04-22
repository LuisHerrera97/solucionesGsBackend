using System;

namespace FinancieraSoluciones.Domain.Enums.Finanzas
{
    public enum EstatusCredito
    {
        Activo,
        Liquidado,
        Reestructurado
    }

    public static class EstatusCreditoExtensions
    {
        public static string ToStoredString(this EstatusCredito estatus) => estatus.ToString();

        public static bool EqualsStored(string? stored, EstatusCredito expected) =>
            string.Equals(stored?.Trim(), expected.ToStoredString(), StringComparison.Ordinal);
    }
}
