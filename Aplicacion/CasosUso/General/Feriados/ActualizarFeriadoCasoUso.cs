using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General.Feriados
{
    public class ActualizarFeriadoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IFeriadoRepositorio _feriadoRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarFeriadoCasoUso(IMapper mapper, IFeriadoRepositorio feriadoRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _feriadoRepositorio = feriadoRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<FeriadoDto> Ejecutar(Guid id, FeriadoDto dto)
        {
            if (dto == null) throw new ArgumentException("Request requerido");
            var entity = await _feriadoRepositorio.GetByIdAsync(id);
            if (entity == null) throw new ArgumentException("No existe el feriado");

            if (entity.Fecha.Date != dto.Fecha.Date)
            {
                var byFecha = await _feriadoRepositorio.GetByFechaAsync(dto.Fecha.Date);
                if (byFecha != null && byFecha.Id != id) throw new ArgumentException("Ya existe un feriado en esa fecha");
                entity.Fecha = dto.Fecha.Date;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) entity.Nombre = dto.Nombre.Trim();
            entity.Activo = dto.Activo;

            await _feriadoRepositorio.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<FeriadoDto>(entity);
        }
    }
}
