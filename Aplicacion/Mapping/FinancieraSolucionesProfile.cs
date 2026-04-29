using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Cobranza;
using FinancieraSoluciones.Application.DTOs.Cobranza.Pendientes;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Finanzas.Cortes;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Cobranza;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Pendientes;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Application.Mapping
{
    public class FinancieraSolucionesProfile : Profile
    {
        public FinancieraSolucionesProfile()
        {
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(d => d.ApellidoMaterno, opt => opt.MapFrom(s => s.ApellidoMaterno ?? string.Empty))
                .ForMember(d => d.NombrePerfil, opt => opt.MapFrom(s => s.Perfil != null ? s.Perfil.Nombre : string.Empty))
                .ForMember(d => d.NombreZonaCobranza, opt => opt.MapFrom(s => s.ZonaCobranza != null ? s.ZonaCobranza.Nombre : string.Empty));

            CreateMap<UsuarioDto, Usuario>()
                .ForMember(d => d.ApellidoMaterno, opt => opt.MapFrom(s => string.IsNullOrWhiteSpace(s.ApellidoMaterno) ? null : s.ApellidoMaterno))
                .ForMember(d => d.Perfil, opt => opt.Ignore())
                .ForMember(d => d.ZonaCobranza, opt => opt.Ignore());
            CreateMap<Perfil, PerfilDto>().ReverseMap();
            CreateMap<Modulo, ModuloDto>().ReverseMap();
            CreateMap<Pagina, PaginaDto>();
            CreateMap<PaginaDto, Pagina>()
                .ForMember(d => d.EnMenu, opt => opt.MapFrom(s => s.EnMenu ?? true));
            CreateMap<Boton, BotonDto>().ReverseMap();

            CreateMap<ConfiguracionSistema, ConfiguracionSistemaDto>().ReverseMap();
            CreateMap<ZonaCobranza, ZonaCobranzaDto>().ReverseMap();
            CreateMap<AuditoriaEvento, AuditoriaEventoDto>()
                .ForMember(d => d.UsuarioNombre, opt => opt.Ignore());
            CreateMap<AuditoriaEventoDto, AuditoriaEvento>();
            CreateMap<Feriado, FeriadoDto>().ReverseMap();

            CreateMap<Cliente, ClienteDto>().ReverseMap();

            CreateMap<Credito, CreditoResumenDto>().ReverseMap();

            CreateMap<Ficha, FichaDto>()
                .ForMember(d => d.Hora, opt => opt.MapFrom(s => s.Hora ?? string.Empty))
                .ReverseMap();

            CreateMap<Credito, CreditoDto>()
                .ForMember(d => d.ClienteNombre, opt => opt.MapFrom(s => s.Cliente != null ? s.Cliente.Nombre : string.Empty))
                .ForMember(d => d.ClienteApellido, opt => opt.MapFrom(s => s.Cliente != null ? s.Cliente.Apellido : string.Empty))
                .ForMember(d => d.ClienteNegocio, opt => opt.MapFrom(s => s.Cliente != null ? (s.Cliente.Negocio ?? string.Empty) : string.Empty))
                .ForMember(d => d.ClienteZona, opt => opt.MapFrom(s => s.Cliente != null ? (s.Cliente.Zona ?? string.Empty) : string.Empty))
                .ForMember(d => d.Fichas, opt => opt.MapFrom(s => s.Fichas));

            CreateMap<CreditoDto, Credito>()
                .ForMember(d => d.Cliente, opt => opt.Ignore())
                .ForMember(d => d.Fichas, opt => opt.MapFrom(s => s.Fichas));

            CreateMap<CorteCaja, CorteCajaDto>()
                .ForMember(d => d.Hora, opt => opt.MapFrom(s => s.Hora ?? string.Empty))
                .ForMember(d => d.Movimientos, opt => opt.MapFrom(s => s.Movimientos));

            CreateMap<CorteCajaDto, CorteCaja>()
                .ForMember(d => d.Movimientos, opt => opt.MapFrom(s => s.Movimientos));

            CreateMap<MovimientoCaja, MovimientoCajaDto>()
                .ForMember(d => d.Hora, opt => opt.MapFrom(s => s.Hora ?? string.Empty))
                .ReverseMap();

            CreateMap<MovimientoCobranza, MovimientoCobranzaDto>().ReverseMap();
            CreateMap<PendienteCobro, PendienteCobroDto>().ReverseMap();
        }
    }
}
