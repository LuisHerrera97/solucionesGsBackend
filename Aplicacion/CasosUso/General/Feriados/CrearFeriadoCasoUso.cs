using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General.Feriados
{
    public class CrearFeriadoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IFeriadoRepositorio _feriadoRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearFeriadoCasoUso(IMapper mapper, IFeriadoRepositorio feriadoRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _feriadoRepositorio = feriadoRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<FeriadoDto> Ejecutar(FeriadoDto dto)
        {
            if (dto == null) throw new ArgumentException("Request requerido");
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre requerido");

            var fecha = dto.Fecha.Date;
            var existing = await _feriadoRepositorio.GetByFechaAsync(fecha);
            if (existing != null) throw new ArgumentException("Ya existe un feriado en esa fecha");

            var entity = new Feriado
            {
                Id = Guid.NewGuid(),
                Fecha = fecha,
                Nombre = dto.Nombre.Trim(),
                Activo = dto.Activo,
                FechaCreacion = DateTime.UtcNow
            };

            await _feriadoRepositorio.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<FeriadoDto>(entity);
        }
    }
}

