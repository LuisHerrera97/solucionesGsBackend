using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.Seguridad;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class RestablecerPasswordCasoUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordResetTokenRepositorio _tokenRepositorio;
        private readonly IPasswordHistoryRepositorio _passwordHistoryRepositorio;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public RestablecerPasswordCasoUso(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordResetTokenRepositorio tokenRepositorio,
            IPasswordHistoryRepositorio passwordHistoryRepositorio,
            IPasswordHasherService passwordHasherService,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _tokenRepositorio = tokenRepositorio;
            _passwordHistoryRepositorio = passwordHistoryRepositorio;
            _passwordHasherService = passwordHasherService;
            _configuracionRepositorio = configuracionRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(RestablecerPasswordRequestDto request)
        {
            var usuarioAcceso = request?.UsuarioAcceso?.Trim();
            var codigo = request?.Codigo?.Trim();
            var nueva = request?.NuevaContrasena;

            if (string.IsNullOrWhiteSpace(usuarioAcceso)) throw new ArgumentException("Usuario requerido");
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Código requerido");
            if (string.IsNullOrWhiteSpace(nueva)) throw new ArgumentException("Contraseña requerida");

            var nowUtc = DateTime.UtcNow;
            var usuario = await _usuarioRepositorio.GetByUsuarioAccesoAsync(usuarioAcceso);
            if (usuario == null || !usuario.Activo) throw new ArgumentException("Código inválido");

            var token = await _tokenRepositorio.GetValidAsync(usuario.Id, codigo, nowUtc);
            if (token == null) throw new ArgumentException("Código inválido");

            var config = await _configuracionRepositorio.GetAsync();
            PasswordPolicy.Validar(config, nueva);

            var historyTake = config != null && config.PasswordHistoryCount > 0 ? config.PasswordHistoryCount : 0;
            if (historyTake > 0)
            {
                var history = (await _passwordHistoryRepositorio.GetRecentAsync(usuario.Id, historyTake)).ToList();
                var reused = history.Any(h => _passwordHasherService.VerifyPassword(nueva, h.PasswordHash));
                if (reused) throw new ArgumentException("No puedes reutilizar una contraseña reciente");
            }

            var newHash = _passwordHasherService.HashPassword(nueva);
            usuario.Contrasena = newHash;
            usuario.PasswordChangedAt = nowUtc;
            usuario.PasswordExpiresAt = config != null && config.PasswordExpireDays > 0 ? nowUtc.AddDays(config.PasswordExpireDays) : null;
            usuario.MustChangePassword = false;
            usuario.FailedLoginCount = 0;
            usuario.LockoutUntil = null;
            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiryTime = null;

            token.UsedAt = nowUtc;
            await _usuarioRepositorio.UpdateAsync(usuario);
            await _passwordHistoryRepositorio.AddAsync(new PasswordHistory
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                PasswordHash = newHash,
                CreatedAt = nowUtc
            });

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Accion = "RestablecerPassword",
                EntidadTipo = "Usuario",
                EntidadId = usuario.Id,
                Fecha = nowUtc,
                Detalle = null
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

