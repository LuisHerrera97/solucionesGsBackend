using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ObtenerClientesCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepositorio _clienteRepositorio;

        public ObtenerClientesCasoUso(IMapper mapper, IClienteRepositorio clienteRepositorio)
        {
            _mapper = mapper;
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ClientesListadoDto> Ejecutar(int? page = null, int? pageSize = null, string? buscar = null, Guid? zonaId = null, bool aplicarFiltroZona = false)
        {
            var (clientes, totalCount) = await _clienteRepositorio.GetAllAsync(page, pageSize, buscar, zonaId, aplicarFiltroZona);
            return new ClientesListadoDto
            {
                Items = clientes.Select(c => _mapper.Map<ClienteDto>(c)).ToList(),
                TotalCount = totalCount
            };
        }
    }
}
