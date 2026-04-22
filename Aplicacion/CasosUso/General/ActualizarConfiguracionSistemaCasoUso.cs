using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ActualizarConfiguracionSistemaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarConfiguracionSistemaCasoUso(IMapper mapper, IConfiguracionSistemaRepositorio configuracionRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuracionRepositorio = configuracionRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ConfiguracionSistemaDto> Ejecutar(ConfiguracionSistemaDto dto)
        {
            if (dto.MoraDiaria < 0 || dto.MoraSemanal < 0 || dto.MoraMensual < 0 || dto.TasaDiaria < 0 || dto.TasaSemanal < 0 || dto.TasaMensual < 0)
            {
                throw new ArgumentException("Los valores no pueden ser negativos");
            }
            if (dto.DiasGraciaDiaria < 0 || dto.DiasGraciaSemanal < 0 || dto.DiasGraciaMensual < 0) throw new ArgumentException("Días de gracia inválidos");
            if (dto.TopeMoraDiaria < 0 || dto.TopeMoraSemanal < 0 || dto.TopeMoraMensual < 0) throw new ArgumentException("Topes de mora inválidos");
            if (dto.PasswordMinLength < 6) throw new ArgumentException("La longitud mínima de contraseña debe ser al menos 6");
            if (dto.PasswordExpireDays < 0) throw new ArgumentException("Días de expiración inválidos");
            if (dto.PasswordHistoryCount < 0) throw new ArgumentException("Historial de contraseñas inválido");
            if (dto.LockoutMaxFailedAttempts < 0) throw new ArgumentException("Intentos máximos inválidos");
            if (dto.LockoutMinutes < 0) throw new ArgumentException("Minutos de bloqueo inválidos");

            var config = await _configuracionRepositorio.GetAsync();
            if (config == null)
            {
                throw new ArgumentException("No existe configuración a actualizar");
            }

            config.MoraDiaria = dto.MoraDiaria;
            config.MoraSemanal = dto.MoraSemanal;
            config.MoraMensual = dto.MoraMensual;
            config.DiasGraciaDiaria = dto.DiasGraciaDiaria;
            config.DiasGraciaSemanal = dto.DiasGraciaSemanal;
            config.DiasGraciaMensual = dto.DiasGraciaMensual;
            config.TopeMoraDiaria = dto.TopeMoraDiaria;
            config.TopeMoraSemanal = dto.TopeMoraSemanal;
            config.TopeMoraMensual = dto.TopeMoraMensual;
            config.TasaDiaria = dto.TasaDiaria;
            config.TasaSemanal = dto.TasaSemanal;
            config.TasaMensual = dto.TasaMensual;
            config.DomingoInhabilDefault = dto.DomingoInhabilDefault;
            config.AplicarFeriadosDefault = dto.AplicarFeriadosDefault;
            config.PasswordMinLength = dto.PasswordMinLength;
            config.PasswordRequireUpper = dto.PasswordRequireUpper;
            config.PasswordRequireLower = dto.PasswordRequireLower;
            config.PasswordRequireDigit = dto.PasswordRequireDigit;
            config.PasswordRequireSpecial = dto.PasswordRequireSpecial;
            config.PasswordExpireDays = dto.PasswordExpireDays;
            config.PasswordHistoryCount = dto.PasswordHistoryCount;
            config.LockoutMaxFailedAttempts = dto.LockoutMaxFailedAttempts;
            config.LockoutMinutes = dto.LockoutMinutes;
            config.FechaActualizacion = DateTime.UtcNow;

            await _configuracionRepositorio.UpdateAsync(config);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ConfiguracionSistemaDto>(config);
        }
    }
}
