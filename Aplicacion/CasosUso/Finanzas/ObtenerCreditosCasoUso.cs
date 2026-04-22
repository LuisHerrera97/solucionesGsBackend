using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ObtenerCreditosCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;

        public ObtenerCreditosCasoUso(IMapper mapper, ICreditoRepositorio creditoRepositorio)
        {
            _mapper = mapper;
            _creditoRepositorio = creditoRepositorio;
        }

        public async Task<IEnumerable<CreditoDto>> Ejecutar(string? searchTerm = null, int? page = null, int? pageSize = null, Guid? zonaId = null, bool aplicarFiltroZona = false)
        {
            var creditos = await _creditoRepositorio.GetAllAsync(searchTerm, page, pageSize, zonaId, aplicarFiltroZona);
            return creditos.Select(c =>
            {
                var dto = _mapper.Map<CreditoDto>(c);
                dto.Fichas = new List<FichaDto>();
                return dto;
            });
        }
    }
}
