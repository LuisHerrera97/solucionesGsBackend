using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ActualizarZonaCobranzaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IZonaCobranzaRepositorio _zonaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarZonaCobranzaCasoUso(IMapper mapper, IZonaCobranzaRepositorio zonaRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _zonaRepositorio = zonaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ZonaCobranzaDto> Ejecutar(Guid id, ZonaCobranzaDto dto)
        {
            var zona = await _zonaRepositorio.GetByIdAsync(id);
            if (zona == null)
            {
                throw new ArgumentException("No existe la zona especificada");
            }

            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new ArgumentException("El nombre de la zona es requerido");
            }

            zona.Nombre = dto.Nombre.Trim();
            zona.Activo = dto.Activo;
            zona.Orden = dto.Orden;

            await _zonaRepositorio.UpdateAsync(zona);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ZonaCobranzaDto>(zona);
        }
    }
}
