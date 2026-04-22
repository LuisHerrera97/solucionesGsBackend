using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class AutenticarUsuarioCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public AutenticarUsuarioCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio,
            ITokenService tokenService,
            IPasswordHasherService passwordHasherService,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
            _tokenService = tokenService;
            _passwordHasherService = passwordHasherService;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<AutenticacionResponseDto> Ejecutar(AutenticacionRequestDto request)
        {
            var nowUtc = DateTime.UtcNow;
            var config = await _configuracionRepositorio.GetAsync();

            var usuario = await _usuarioRepositorio.GetByUsuarioAccesoAsync(request.UsuarioAcceso);
            
            if (usuario == null || !usuario.Activo)
            {
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = null,
                    Accion = "LoginFallido",
                    EntidadTipo = "Usuario",
                    EntidadId = null,
                    Fecha = nowUtc,
                    Detalle = request?.UsuarioAcceso
                });
                await _unitOfWork.SaveChangesAsync();
                return new AutenticacionResponseDto
                {
                    Autenticado = false,
                    Token = null,
                    RefreshToken = null,
                    Usuario = null
                };
            }

            if (usuario.LockoutUntil.HasValue && usuario.LockoutUntil.Value > nowUtc)
            {
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Accion = "LoginBloqueado",
                    EntidadTipo = "Usuario",
                    EntidadId = usuario.Id,
                    Fecha = nowUtc,
                    Detalle = usuario.LockoutUntil.Value.ToString("O")
                });
                await _unitOfWork.SaveChangesAsync();
                throw new ArgumentException("Usuario bloqueado temporalmente");
            }

            if (!await VerificarPasswordYActualizarSiEsNecesario(usuario, request.Contrasena))
            {
                usuario.FailedLoginCount += 1;
                var max = config != null && config.LockoutMaxFailedAttempts > 0 ? config.LockoutMaxFailedAttempts : 5;
                var mins = config != null && config.LockoutMinutes > 0 ? config.LockoutMinutes : 15;
                if (usuario.FailedLoginCount >= max)
                {
                    usuario.LockoutUntil = nowUtc.AddMinutes(mins);
                    usuario.FailedLoginCount = 0;
                }

                await _usuarioRepositorio.UpdateAsync(usuario);
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Accion = "LoginFallido",
                    EntidadTipo = "Usuario",
                    EntidadId = usuario.Id,
                    Fecha = nowUtc,
                    Detalle = request?.UsuarioAcceso
                });
                await _unitOfWork.SaveChangesAsync();
                return new AutenticacionResponseDto
                {
                    Autenticado = false,
                    Token = null,
                    RefreshToken = null,
                    Usuario = null
                };
            }

            usuario.FailedLoginCount = 0;
            usuario.LockoutUntil = null;

            if (usuario.MustChangePassword)
            {
                await _usuarioRepositorio.UpdateAsync(usuario);
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Accion = "LoginRequiereCambioPassword",
                    EntidadTipo = "Usuario",
                    EntidadId = usuario.Id,
                    Fecha = nowUtc,
                    Detalle = null
                });
                await _unitOfWork.SaveChangesAsync();
                throw new ArgumentException("Debes cambiar tu contraseña");
            }

            if (usuario.PasswordExpiresAt.HasValue && usuario.PasswordExpiresAt.Value <= nowUtc)
            {
                await _usuarioRepositorio.UpdateAsync(usuario);
                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Accion = "LoginPasswordExpirada",
                    EntidadTipo = "Usuario",
                    EntidadId = usuario.Id,
                    Fecha = nowUtc,
                    Detalle = usuario.PasswordExpiresAt.Value.ToString("O")
                });
                await _unitOfWork.SaveChangesAsync();
                throw new ArgumentException("Contraseña expirada");
            }

            var perfil = await _perfilRepositorio.GetByIdAsync(usuario.IdPerfil);
            
            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            usuarioDto.NombrePerfil = perfil?.Nombre ?? string.Empty;
            if (usuario.IdZonaCobranza.HasValue)
            {
                var zonas = await _zonaCobranzaRepositorio.GetAllAsync();
                var zona = zonas.FirstOrDefault(z => z.Id == usuario.IdZonaCobranza.Value);
                usuarioDto.NombreZonaCobranza = zona?.Nombre ?? string.Empty;
            }

            var token = _tokenService.GenerateAccessToken(usuario);
            var refreshToken = _tokenService.GenerateRefreshToken();

            usuario.UltimoAcceso = nowUtc;
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = nowUtc.AddDays(7);
            await _usuarioRepositorio.UpdateAsync(usuario);

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Accion = "LoginExitoso",
                EntidadTipo = "Usuario",
                EntidadId = usuario.Id,
                Fecha = nowUtc,
                Detalle = null
            });
            await _unitOfWork.SaveChangesAsync();

            return new AutenticacionResponseDto
            {
                Autenticado = true,
                Token = token,
                RefreshToken = refreshToken,
                Usuario = usuarioDto
            };
        }

        private async Task<bool> VerificarPasswordYActualizarSiEsNecesario(Usuario usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                return false;
            }

            var isBcryptHash =
                usuario.Contrasena.StartsWith("$2a$") ||
                usuario.Contrasena.StartsWith("$2b$") ||
                usuario.Contrasena.StartsWith("$2x$") ||
                usuario.Contrasena.StartsWith("$2y$");

            if (isBcryptHash)
            {
                return _passwordHasherService.VerifyPassword(password, usuario.Contrasena);
            }

            if (!string.Equals(usuario.Contrasena, password, StringComparison.Ordinal))
            {
                return false;
            }

            usuario.Contrasena = _passwordHasherService.HashPassword(password);
            await _usuarioRepositorio.UpdateAsync(usuario);
            return true;
        }
    }
}
