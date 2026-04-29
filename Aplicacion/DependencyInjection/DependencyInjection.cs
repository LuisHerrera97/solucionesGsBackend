using FinancieraSoluciones.Application.CasosUso.General;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Caja;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Cortes;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Pendientes;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Cobranza;
using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.Servicios.Finanzas;
using Microsoft.Extensions.DependencyInjection;

namespace FinancieraSoluciones.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFinancieraSolucionesApplication(this IServiceCollection services)
        {
            services.AddScoped<AutenticarUsuarioCasoUso>();
            services.AddScoped<RefrescarTokenCasoUso>();
            services.AddScoped<SolicitarRecuperacionPasswordCasoUso>();
            services.AddScoped<RestablecerPasswordCasoUso>();
            services.AddScoped<CambiarPasswordCasoUso>();
            services.AddScoped<CrearUsuarioCasoUso>();
            services.AddScoped<ObtenerUsuariosCasoUso>();
            services.AddScoped<ObtenerUsuarioPorIdCasoUso>();
            services.AddScoped<ActualizarUsuarioCasoUso>();
            services.AddScoped<EliminarUsuarioCasoUso>();
            services.AddScoped<ResetPasswordAdminCasoUso>();
            services.AddScoped<CrearPerfilCasoUso>();
            services.AddScoped<ActualizarPerfilCasoUso>();
            services.AddScoped<EliminarPerfilCasoUso>();
            services.AddScoped<ObtenerPerfilesCasoUso>();
            services.AddScoped<ObtenerModulosCasoUso>();
            services.AddScoped<CrearModuloCasoUso>();
            services.AddScoped<ActualizarModuloCasoUso>();
            services.AddScoped<EliminarModuloCasoUso>();
            services.AddScoped<ObtenerPaginasCasoUso>();
            services.AddScoped<CrearPaginaCasoUso>();
            services.AddScoped<ActualizarPaginaCasoUso>();
            services.AddScoped<EliminarPaginaCasoUso>();
            services.AddScoped<ObtenerBotonesCasoUso>();
            services.AddScoped<CrearBotonCasoUso>();
            services.AddScoped<ActualizarBotonCasoUso>();
            services.AddScoped<EliminarBotonCasoUso>();
            services.AddScoped<ObtenerModulosConPermisosCasoUso>();
            services.AddScoped<AsignarPermisosCasoUso>();
            services.AddScoped<ObtenerPermisosPorPerfilCasoUso>();
            services.AddScoped<ObtenerConfiguracionSistemaCasoUso>();
            services.AddScoped<ActualizarConfiguracionSistemaCasoUso>();
            services.AddScoped<ObtenerZonasCobranzaCasoUso>();
            services.AddScoped<ObtenerAuditoriaCasoUso>();
            services.AddScoped<ObtenerAuditoriaFiltrosOpcionesCasoUso>();
            services.AddScoped<FinancieraSoluciones.Application.CasosUso.General.Feriados.ObtenerFeriadosCasoUso>();
            services.AddScoped<FinancieraSoluciones.Application.CasosUso.General.Feriados.CrearFeriadoCasoUso>();
            services.AddScoped<FinancieraSoluciones.Application.CasosUso.General.Feriados.ActualizarFeriadoCasoUso>();
            services.AddScoped<FinancieraSoluciones.Application.CasosUso.General.Feriados.EliminarFeriadoCasoUso>();
            services.AddScoped<CrearZonaCobranzaCasoUso>();
            services.AddScoped<ActualizarZonaCobranzaCasoUso>();
            services.AddScoped<EliminarZonaCobranzaCasoUso>();
            services.AddScoped<ObtenerClientesCasoUso>();
            services.AddScoped<CrearClienteCasoUso>();
            services.AddScoped<ActualizarClienteCasoUso>();
            services.AddScoped<EliminarClienteCasoUso>();
            services.AddScoped<ObtenerCreditosPorClienteCasoUso>();
            services.AddScoped<ObtenerCreditosCasoUso>();
            services.AddScoped<ObtenerResumenDashboardCasoUso>();
            services.AddScoped<ObtenerCreditoPorIdCasoUso>();
            services.AddScoped<CrearCreditoCasoUso>();
            services.AddScoped<AbonarFichaCasoUso>();
            services.AddScoped<ActualizarMoraAcumuladaCasoUso>();
            services.AddScoped<ReestructurarCreditoCasoUso>();
            services.AddScoped<ObtenerPendientesCasoUso>();
            services.AddScoped<ObtenerCobranzaCasoUso>();
            services.AddScoped<ObtenerMovimientosEnRangoCasoUso>();
            services.AddScoped<ObtenerMovimientosPorCreditoCasoUso>();

            services.AddScoped<MarcarMovimientosRecibidoCajaCasoUso>();
            services.AddScoped<ObtenerCortesCasoUso>();
            services.AddScoped<CondonarInteresFichaCasoUso>();
            services.AddScoped<CondonarInteresMontoCasoUso>();
            services.AddScoped<ActualizarObservacionCasoUso>();
            services.AddScoped<PenalizarFichaManualCasoUso>();
            services.AddScoped<ReversarOperacionMovimientoCasoUso>();
            services.AddScoped<ICreditoZonaAutorizacionService, CreditoZonaAutorizacionService>();
            return services;
        }
    }
}
