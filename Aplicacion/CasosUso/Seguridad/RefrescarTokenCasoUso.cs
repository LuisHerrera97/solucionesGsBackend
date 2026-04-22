using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class RefrescarTokenCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefrescarTokenCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AutenticacionResponseDto> Ejecutar(RefreshTokenRequestDto request)
        {
            if (request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new ArgumentException("Refresh token inválido");
            }

            var usuario = await _usuarioRepositorio.GetByIdAsync(request.UserId);
            if (usuario == null || !usuario.Activo)
            {
                throw new ArgumentException("Refresh token inválido");
            }

            if (string.IsNullOrWhiteSpace(usuario.RefreshToken) ||
                !string.Equals(usuario.RefreshToken, request.RefreshToken, StringComparison.Ordinal) ||
                usuario.RefreshTokenExpiryTime == null ||
                usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new ArgumentException("Refresh token expirado o inválido");
            }

            var token = _tokenService.GenerateAccessToken(usuario);
            var refreshToken = _tokenService.GenerateRefreshToken();

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _usuarioRepositorio.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            var perfil = await _perfilRepositorio.GetByIdAsync(usuario.IdPerfil);
            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            usuarioDto.NombrePerfil = perfil?.Nombre ?? string.Empty;

            return new AutenticacionResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Usuario = usuarioDto,
                Autenticado = true
            };
        }
    }
}
