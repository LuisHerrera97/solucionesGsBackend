using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General.Feriados
{
    public class ObtenerFeriadosCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IFeriadoRepositorio _feriadoRepositorio;

        public ObtenerFeriadosCasoUso(IMapper mapper, IFeriadoRepositorio feriadoRepositorio)
        {
            _mapper = mapper;
            _feriadoRepositorio = feriadoRepositorio;
        }

        public async Task<IEnumerable<FeriadoDto>> Ejecutar()
        {
            var items = await _feriadoRepositorio.GetAllAsync();
            return items.Select(f => _mapper.Map<FeriadoDto>(f)).ToList();
        }
    }
}

