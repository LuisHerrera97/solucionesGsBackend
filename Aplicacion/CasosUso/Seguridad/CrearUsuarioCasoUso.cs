using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class CrearUsuarioCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IPasswordHistoryRepositorio _passwordHistoryRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearUsuarioCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio,
            IPasswordHasherService passwordHasherService,
            IPasswordHistoryRepositorio passwordHistoryRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
            _passwordHasherService = passwordHasherService;
            _passwordHistoryRepositorio = passwordHistoryRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Ejecutar(UsuarioCrearDto usuarioDto)
        {
            var perfil = await _perfilRepositorio.GetByIdAsync(usuarioDto.IdPerfil);
            if (perfil == null)
            {
                throw new ArgumentException("El perfil especificado no existe");
            }

            if (await _usuarioRepositorio.ExistsByUsuarioAccesoAsync(usuarioDto.UsuarioAcceso))
            {
                throw new ArgumentException("El nombre de usuario ya está en uso");
            }

            if (string.IsNullOrWhiteSpace(usuarioDto.Contrasena))
            {
                throw new ArgumentException("La contraseña es requerida");
            }

            var config = await _configuracionRepositorio.GetAsync();
            PasswordPolicy.Validar(config, usuarioDto.Contrasena);

            var usuario = _mapper.Map<Usuario>(usuarioDto);
            usuario.Id = Guid.NewGuid();
            var passwordHash = _passwordHasherService.HashPassword(usuarioDto.Contrasena);
            usuario.Contrasena = passwordHash;
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.UltimoAcceso = null;
            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiryTime = null;
            usuario.PasswordChangedAt = DateTime.UtcNow;
            usuario.PasswordExpiresAt = config != null && config.PasswordExpireDays > 0 ? DateTime.UtcNow.AddDays(config.PasswordExpireDays) : null;
            usuario.MustChangePassword = false;
            usuario.FailedLoginCount = 0;
            usuario.LockoutUntil = null;

            var usuarioGuardado = await _usuarioRepositorio.AddAsync(usuario);
            await _passwordHistoryRepositorio.AddAsync(new PasswordHistory
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioGuardado.Id,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();

            var usuarioRespuesta = _mapper.Map<UsuarioDto>(usuarioGuardado);
            usuarioRespuesta.NombrePerfil = perfil.Nombre;

            return usuarioRespuesta;
        }
    }
}
