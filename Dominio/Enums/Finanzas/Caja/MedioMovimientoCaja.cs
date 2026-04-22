using System;

namespace FinancieraSoluciones.Domain.Enums.Finanzas.Caja
{
    public enum MedioMovimientoCaja
    {
        Efectivo,
        Tarjeta,
        Transferencia,
        Mixto,
        Ajuste
    }

    public static class MedioMovimientoCajaExtensions
    {
        public static string ToStoredString(this MedioMovimientoCaja medio) => medio.ToString();

        public static bool TryParseFromStored(string? value, out MedioMovimientoCaja medio)
        {
            medio = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return Enum.TryParse(value.Trim(), ignoreCase: true, out medio);
        }

        public static bool EqualsStored(string? stored, MedioMovimientoCaja expected) =>
            string.Equals(stored?.Trim(), expected.ToStoredString(), StringComparison.Ordinal);
    }
}
