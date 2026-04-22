using System;

namespace FinancieraSoluciones.Domain.Entidades.General
{
    public class ConfiguracionSistema
    {
        public Guid Id { get; set; }
        public decimal MoraDiaria { get; set; }
        public decimal MoraSemanal { get; set; }
        public int DiasGraciaDiaria { get; set; }
        public int DiasGraciaSemanal { get; set; }
        public decimal TopeMoraDiaria { get; set; }
        public decimal TopeMoraSemanal { get; set; }
        public decimal TasaDiaria { get; set; }
        public decimal TasaSemanal { get; set; }
        public decimal MoraMensual { get; set; }
        public int DiasGraciaMensual { get; set; }
        public decimal TopeMoraMensual { get; set; }
        public decimal TasaMensual { get; set; }
        public bool DomingoInhabilDefault { get; set; }
        public bool AplicarFeriadosDefault { get; set; }
        public int PasswordMinLength { get; set; }
        public bool PasswordRequireUpper { get; set; }
        public bool PasswordRequireLower { get; set; }
        public bool PasswordRequireDigit { get; set; }
        public bool PasswordRequireSpecial { get; set; }
        public int PasswordExpireDays { get; set; }
        public int PasswordHistoryCount { get; set; }
        public int LockoutMaxFailedAttempts { get; set; }
        public int LockoutMinutes { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
