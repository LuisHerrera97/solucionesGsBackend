using System;

namespace FinancieraSoluciones.Domain.Enums.Finanzas.Caja
{
    public enum TipoMovimientoCaja
    {
        Ficha,
        Ingreso,
        Entrada,
        Reversa,
        Penalizacion,
        CondonacionInteres
    }

    public static class TipoMovimientoCajaExtensions
    {
        public static string ToStoredString(this TipoMovimientoCaja tipo) => tipo switch
        {
            TipoMovimientoCaja.Penalizacion => "Penalización",
            TipoMovimientoCaja.CondonacionInteres => "CondonacionInteres",
            _ => tipo.ToString()
        };

        public static bool TryParseFromStored(string? value, out TipoMovimientoCaja tipo)
        {
            tipo = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            var v = value.Trim();
            if (string.Equals(v, "Penalización", StringComparison.Ordinal))
            {
                tipo = TipoMovimientoCaja.Penalizacion;
                return true;
            }

            if (string.Equals(v, "CondonacionInteres", StringComparison.OrdinalIgnoreCase))
            {
                tipo = TipoMovimientoCaja.CondonacionInteres;
                return true;
            }

            return Enum.TryParse(v, ignoreCase: true, out tipo);
        }

        public static bool EqualsStored(string? stored, TipoMovimientoCaja expected) =>
            string.Equals(stored?.Trim(), expected.ToStoredString(), StringComparison.Ordinal);
    }
}
