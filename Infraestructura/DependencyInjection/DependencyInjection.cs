using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Pendientes;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Cobranza;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Infraestructura.Repositorios.General;
using FinancieraSoluciones.Infraestructura.Repositorios.Finanzas;
using FinancieraSoluciones.Infraestructura.Repositorios.Finanzas.Caja;
using FinancieraSoluciones.Infraestructura.Repositorios.Finanzas.Cortes;
using FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Pendientes;
using FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Cobranza;
using FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Liquidaciones;
using FinancieraSoluciones.Infraestructura.Repositorios.Seguridad;
using FinancieraSoluciones.Infraestructura.Data;
using FinancieraSoluciones.Infraestructura.Servicios;
using FinancieraSoluciones.Infraestructura.Servicios.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancieraSoluciones.Infraestructura.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFinancieraSolucionesInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IPerfilRepositorio, PerfilRepositorio>();
            services.AddScoped<IModuloRepositorio, ModuloRepositorio>();
            services.AddScoped<IPaginaRepositorio, PaginaRepositorio>();
            services.AddScoped<IBotonRepositorio, BotonRepositorio>();
            services.AddScoped<IPermisoModuloRepositorio, PermisoModuloRepositorio>();
            services.AddScoped<IPermisoPaginaRepositorio, PermisoPaginaRepositorio>();
            services.AddScoped<IPermisoBotonRepositorio, PermisoBotonRepositorio>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IPasswordResetTokenRepositorio, PasswordResetTokenRepositorio>();
            services.AddScoped<IPasswordHistoryRepositorio, PasswordHistoryRepositorio>();
            services.AddScoped<IConfiguracionSistemaRepositorio, ConfiguracionSistemaRepositorio>();
            services.AddScoped<IZonaCobranzaRepositorio, ZonaCobranzaRepositorio>();
            services.AddScoped<IFeriadoRepositorio, FeriadoRepositorio>();
            services.AddScoped<IAuditoriaEventoRepositorio, AuditoriaEventoRepositorio>();
            services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
            services.AddScoped<ICreditoRepositorio, CreditoRepositorio>();
            services.AddScoped<IMovimientoCajaRepositorio, MovimientoCajaRepositorio>();
            services.AddScoped<ICorteCajaRepositorio, CorteCajaRepositorio>();
            services.AddScoped<IPendientesRepositorio, PendientesRepositorio>();
            services.AddScoped<ICobranzaRepositorio, CobranzaRepositorio>();
            services.AddScoped<ILiquidacionCobranzaRepositorio, LiquidacionCobranzaRepositorio>();
            services.AddScoped<IClock, SystemClock>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
