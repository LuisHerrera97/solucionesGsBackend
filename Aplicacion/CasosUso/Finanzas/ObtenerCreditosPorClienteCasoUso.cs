using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ObtenerCreditosPorClienteCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ICreditoRepositorio _creditoRepositorio;

        public ObtenerCreditosPorClienteCasoUso(IMapper mapper, IClienteRepositorio clienteRepositorio, ICreditoRepositorio creditoRepositorio)
        {
            _mapper = mapper;
            _clienteRepositorio = clienteRepositorio;
            _creditoRepositorio = creditoRepositorio;
        }

        public async Task<ClienteCreditosDto> Ejecutar(Guid clienteId)
        {
            var cliente = await _clienteRepositorio.GetByIdAsync(clienteId);
            if (cliente == null) throw new ArgumentException("No existe el cliente");

            var creditos = await _creditoRepositorio.GetByClienteIdAsync(clienteId);

            var dto = new ClienteCreditosDto
            {
                Cliente = _mapper.Map<ClienteDto>(cliente),
                Vigentes = creditos
                    .Where(c => !EstatusCreditoExtensions.EqualsStored(c.Estatus, EstatusCredito.Liquidado))
                    .Select(c => _mapper.Map<CreditoResumenDto>(c))
                    .ToList(),
                Liquidados = creditos
                    .Where(c => EstatusCreditoExtensions.EqualsStored(c.Estatus, EstatusCredito.Liquidado))
                    .Select(c => _mapper.Map<CreditoResumenDto>(c))
                    .ToList()
            };

            return dto;
        }
    }
}

