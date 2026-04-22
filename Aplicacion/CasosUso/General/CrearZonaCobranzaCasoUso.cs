using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class CrearZonaCobranzaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IZonaCobranzaRepositorio _zonaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearZonaCobranzaCasoUso(IMapper mapper, IZonaCobranzaRepositorio zonaRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _zonaRepositorio = zonaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ZonaCobranzaDto> Ejecutar(ZonaCobranzaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new ArgumentException("El nombre de la zona es requerido");
            }

            var exists = await _zonaRepositorio.ExistsByNombreAsync(dto.Nombre.Trim());
            if (exists)
            {
                throw new ArgumentException("Ya existe una zona con ese nombre");
            }

            var zona = _mapper.Map<ZonaCobranza>(dto);
            zona.Id = Guid.NewGuid();
            zona.Nombre = dto.Nombre.Trim();
            zona.Activo = true;
            zona.FechaCreacion = DateTime.UtcNow;

            var created = await _zonaRepositorio.AddAsync(zona);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ZonaCobranzaDto>(created);
        }
    }
}
