using FluentValidation;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;

namespace FinancieraSoluciones.Application.Validaciones.Finanzas
{
    public class AbonarFichaRequestDtoValidator : AbstractValidator<AbonarFichaRequestDto>
    {
        public AbonarFichaRequestDtoValidator()
        {
            RuleFor(x => x.Medio)
                .Must(EsMedioValido)
                .WithMessage("Medio inválido.");

            RuleFor(x => x.MontoAbono)
                .GreaterThan(0m)
                .When(x => x.MontoAbono.HasValue)
                .WithMessage("El monto debe ser mayor a 0.");

            RuleFor(x => x)
                .Must(x => x.MontoEfectivo is null || x.MontoEfectivo >= 0m)
                .WithMessage("Montos de pago inválidos.");
            RuleFor(x => x)
                .Must(x => x.MontoTransferencia is null || x.MontoTransferencia >= 0m)
                .WithMessage("Montos de pago inválidos.");

            When(x => EsMixto(x.Medio), () =>
            {
                RuleFor(x => x.MontoEfectivo).NotNull().WithMessage("En Mixto, montoEfectivo es requerido.");
                RuleFor(x => x.MontoTransferencia).NotNull().WithMessage("En Mixto, montoTransferencia es requerido.");
            });
        }

        private static bool EsMixto(string? medio) =>
            MedioMovimientoCajaExtensions.TryParseFromStored(medio, out var m) && m == MedioMovimientoCaja.Mixto;

        private static bool EsMedioValido(string? medio)
        {
            if (string.IsNullOrWhiteSpace(medio)) return true;
            return MedioMovimientoCajaExtensions.TryParseFromStored(medio, out var parsed)
                && parsed is MedioMovimientoCaja.Efectivo or MedioMovimientoCaja.Transferencia or MedioMovimientoCaja.Mixto;
        }
    }
}
