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
    public class ResetPasswordAdminCasoUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHistoryRepositorio _passwordHistoryRepositorio;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordAdminCasoUso(
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

        public async Task Ejecutar(Guid targetUsuarioId, Guid? adminUsuarioId, ResetPasswordAdminRequestDto request)
        {
            var nueva = request?.NuevaContrasena;

            if (string.IsNullOrWhiteSpace(nueva)) throw new ArgumentException("Nueva contraseña requerida");

            var usuario = await _usuarioRepositorio.GetByIdAsync(targetUsuarioId);
            if (usuario == null) throw new ArgumentException("Usuario no encontrado");

            var nowUtc = DateTime.UtcNow;
            var config = await _configuracionRepositorio.GetAsync();
            PasswordPolicy.Validar(config, nueva);

            // Nota: Podríamos verificar historial aquí si se desea, por ahora permitimos reset abierto por admin
            
            var newHash = _passwordHasherService.HashPassword(nueva);
            usuario.Contrasena = newHash;
            usuario.PasswordChangedAt = nowUtc;
            usuario.PasswordExpiresAt = config != null && config.PasswordExpireDays > 0 ? nowUtc.AddDays(config.PasswordExpireDays) : null;
            usuario.MustChangePassword = true; // Forzamos cambio al siguiente login
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
                UsuarioId = adminUsuarioId,
                Accion = "ResetPasswordAdmin",
                EntidadTipo = "Usuario",
                EntidadId = usuario.Id,
                Fecha = nowUtc,
                Detalle = $"Restablecimiento de contraseña por administrador para usuario: {usuario.UsuarioAcceso}"
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
