using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ActualizarClienteCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarClienteCasoUso(
            IMapper mapper,
            IClienteRepositorio clienteRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _clienteRepositorio = clienteRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClienteDto> Ejecutar(Guid id, ClienteDto dto, Guid? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new ArgumentException("El nombre es requerido");
            }

            if (string.IsNullOrWhiteSpace(dto.Apellido))
            {
                throw new ArgumentException("El apellido es requerido");
            }

            var cliente = await _clienteRepositorio.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new ArgumentException("No existe el cliente");
            }

            cliente.Nombre = dto.Nombre.Trim();
            cliente.Apellido = dto.Apellido.Trim();
            cliente.Direccion = (dto.Direccion ?? string.Empty).Trim();
            cliente.Negocio = (dto.Negocio ?? string.Empty).Trim();
            cliente.Zona = (dto.Zona ?? string.Empty).Trim();
            cliente.IdZona = dto.IdZona;
            cliente.Estatus = string.IsNullOrWhiteSpace(dto.Estatus) ? "Activo" : dto.Estatus.Trim();

            await _clienteRepositorio.UpdateAsync(cliente);

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Accion = "ActualizarCliente",
                EntidadTipo = "Cliente",
                EntidadId = cliente.Id,
                Fecha = _clock.UtcNow,
                Detalle = $"Nombre:{cliente.Nombre} {cliente.Apellido}"
            });

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ClienteDto>(cliente);
        }
    }
}
