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
    public class CambiarPasswordCasoUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHistoryRepositorio _passwordHistoryRepositorio;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CambiarPasswordCasoUso(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordHistoryRepositorio passwordHistoryRepositorio,
            IPasswordHasherService passwordHasherService,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _passwordHistoryRepositorio = passwordHistoryRepositorio;
            _passwordHasherService = passwordHasherService;
            _configuracionRepositorio = configuracionRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid usuarioId, CambiarPasswordRequestDto request)
        {
            var actual = request?.ContrasenaActual;
            var nueva = request?.NuevaContrasena;

            if (string.IsNullOrWhiteSpace(actual)) throw new ArgumentException("Contraseña actual requerida");
            if (string.IsNullOrWhiteSpace(nueva)) throw new ArgumentException("Nueva contraseña requerida");

            var usuario = await _usuarioRepositorio.GetByIdAsync(usuarioId);
            if (usuario == null || !usuario.Activo) throw new ArgumentException("Usuario no válido");

            if (!_passwordHasherService.VerifyPassword(actual, usuario.Contrasena))
            {
                throw new ArgumentException("Contraseña actual incorrecta");
            }

            var nowUtc = DateTime.UtcNow;
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
            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiryTime = null;

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
                Accion = "CambiarPassword",
                EntidadTipo = "Usuario",
                EntidadId = usuario.Id,
                Fecha = nowUtc,
                Detalle = null
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

