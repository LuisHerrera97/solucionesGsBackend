using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ObtenerConfiguracionSistemaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ObtenerConfiguracionSistemaCasoUso(IMapper mapper, IConfiguracionSistemaRepositorio configuracionRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuracionRepositorio = configuracionRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ConfiguracionSistemaDto> Ejecutar()
        {
            var config = await _configuracionRepositorio.GetAsync();
            if (config == null)
            {
                config = new ConfiguracionSistema
                {
                    Id = Guid.NewGuid(),
                    MoraDiaria = 20,
                    MoraSemanal = 150,
                    DiasGraciaDiaria = 0,
                    DiasGraciaSemanal = 0,
                    TopeMoraDiaria = 0,
                    TopeMoraSemanal = 0,
                    TasaDiaria = 0.15m,
                    TasaSemanal = 0.2m,
                    DomingoInhabilDefault = true,
                    AplicarFeriadosDefault = false,
                    PasswordMinLength = 8,
                    PasswordRequireUpper = true,
                    PasswordRequireLower = true,
                    PasswordRequireDigit = true,
                    PasswordRequireSpecial = false,
                    PasswordExpireDays = 90,
                    PasswordHistoryCount = 3,
                    LockoutMaxFailedAttempts = 5,
                    LockoutMinutes = 15,
                    FechaActualizacion = DateTime.UtcNow
                };
                await _configuracionRepositorio.AddAsync(config);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<ConfiguracionSistemaDto>(config);
        }
    }
}
