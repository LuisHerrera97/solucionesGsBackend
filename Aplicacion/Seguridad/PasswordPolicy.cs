using System;
using System.Linq;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Application.Seguridad
{
    public static class PasswordPolicy
    {
        public static void Validar(ConfiguracionSistema config, string password)
        {
            var p = password ?? string.Empty;
            var min = config?.PasswordMinLength > 0 ? config.PasswordMinLength : 8;

            if (p.Length < min) throw new ArgumentException($"La contraseña debe tener al menos {min} caracteres");
            if (config?.PasswordRequireUpper == true && !p.Any(char.IsUpper)) throw new ArgumentException("La contraseña debe incluir una mayúscula");
            if (config?.PasswordRequireLower == true && !p.Any(char.IsLower)) throw new ArgumentException("La contraseña debe incluir una minúscula");
            if (config?.PasswordRequireDigit == true && !p.Any(char.IsDigit)) throw new ArgumentException("La contraseña debe incluir un número");
            if (config?.PasswordRequireSpecial == true && p.All(char.IsLetterOrDigit)) throw new ArgumentException("La contraseña debe incluir un caracter especial");
        }
    }
}

