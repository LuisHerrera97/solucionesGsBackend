using FluentValidation;
using FinancieraSoluciones.Application.DTOs.Finanzas;

namespace FinancieraSoluciones.Application.Validaciones.Finanzas
{
    public class PenalizarFichaRequestDtoValidator : AbstractValidator<PenalizarFichaRequestDto>
    {
        public PenalizarFichaRequestDtoValidator()
        {
            RuleFor(x => x.Monto).GreaterThan(0m).WithMessage("El monto debe ser mayor a 0.");
        }
    }
}
