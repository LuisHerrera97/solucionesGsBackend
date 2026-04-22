using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class SolicitarRecuperacionPasswordCasoUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordResetTokenRepositorio _tokenRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public SolicitarRecuperacionPasswordCasoUso(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordResetTokenRepositorio tokenRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _tokenRepositorio = tokenRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<SolicitarRecuperacionPasswordResponseDto> Ejecutar(SolicitarRecuperacionPasswordRequestDto request)
        {
            var usuarioAcceso = request?.UsuarioAcceso?.Trim();
            if (string.IsNullOrWhiteSpace(usuarioAcceso)) throw new ArgumentException("Usuario requerido");

            var nowUtc = DateTime.UtcNow;
            var usuario = await _usuarioRepositorio.GetByUsuarioAccesoAsync(usuarioAcceso);
            if (usuario == null || !usuario.Activo)
            {
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = null,
                    Accion = "SolicitarRecuperacionPassword",
                    EntidadTipo = "Usuario",
                    EntidadId = null,
                    Fecha = nowUtc,
                    Detalle = usuarioAcceso
                });
                await _unitOfWork.SaveChangesAsync();
                return new SolicitarRecuperacionPasswordResponseDto { Ok = true, Codigo = null };
            }

            var codigo = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            await _tokenRepositorio.InvalidateAllAsync(usuario.Id, nowUtc);
            await _tokenRepositorio.AddAsync(new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Codigo = codigo,
                ExpiresAt = nowUtc.AddMinutes(15),
                UsedAt = null,
                CreatedAt = nowUtc
            });

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Accion = "SolicitarRecuperacionPassword",
                EntidadTipo = "Usuario",
                EntidadId = usuario.Id,
                Fecha = nowUtc,
                Detalle = null
            });

            usuario.MustChangePassword = true;
            await _usuarioRepositorio.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _ = await _configuracionRepositorio.GetAsync();

            return new SolicitarRecuperacionPasswordResponseDto { Ok = true, Codigo = codigo };
        }
    }
}

