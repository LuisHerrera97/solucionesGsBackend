using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ObtenerCreditoPorIdCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;

        public ObtenerCreditoPorIdCasoUso(IMapper mapper, ICreditoRepositorio creditoRepositorio)
        {
            _mapper = mapper;
            _creditoRepositorio = creditoRepositorio;
        }

        public async Task<CreditoDto> Ejecutar(Guid id)
        {
            var credito = await _creditoRepositorio.GetByIdAsync(id);
            if (credito == null) return null;

            var dto = _mapper.Map<CreditoDto>(credito);
            dto.Fichas = credito.Fichas
                .OrderBy(f => f.Num)
                .Select(f => _mapper.Map<FichaDto>(f))
                .ToList();
            return dto;
        }
    }
}
