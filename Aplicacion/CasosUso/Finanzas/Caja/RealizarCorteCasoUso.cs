using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class RealizarCorteCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly ICorteCajaRepositorio _corteRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarCorteCasoUso(
            IMapper mapper,
            IMovimientoCajaRepositorio movimientoRepositorio,
            ICorteCajaRepositorio corteRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
            _corteRepositorio = corteRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<CorteCajaDto> Ejecutar(Guid usuarioId, RealizarCorteRequestDto request)
        {
            if (request == null) throw new ArgumentException("Request requerido");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var fechaCorte = request.FechaCorte?.Date ?? DateTime.Today;


                var movimientosTurno = await _movimientoRepositorio.ObtenerTurnoAsync(fechaCorte);
                var totalTeorico = movimientosTurno.Sum(m => m.Total);
                var totalReal = request.TotalReal;
                var diferencia = totalReal - totalTeorico;

                var folio = await _corteRepositorio.ObtenerSiguienteFolioAsync(fechaCorte);
                var corte = new CorteCaja
                {
                    Id = Guid.NewGuid(),
                    Folio = folio,
                    Fecha = fechaCorte,
                    Hora = DateTime.Now.ToString("HH:mm"),
                    TotalTeorico = totalTeorico,
                    TotalReal = totalReal,
                    Diferencia = diferencia,
                    RealizadoPorId = usuarioId
                };

                var created = await _corteRepositorio.AddAsync(corte);
                await _movimientoRepositorio.AsignarCorteAsync(created.Id, created.Fecha);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "RealizarCorteCaja",
                    EntidadTipo = "CorteCaja",
                    EntidadId = created.Id,
                    Fecha = DateTime.UtcNow,
                    Detalle = $"Folio:{created.Folio};Teorico:{totalTeorico};Real:{totalReal};Diferencia:{diferencia}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<CorteCajaDto>(created);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
