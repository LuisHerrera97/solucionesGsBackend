using System;
using System.Threading;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinancieraSoluciones.Api.BackgroundJobs
{
    public class ActualizarMoraBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ActualizarMoraBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var next = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
                if (next <= now) next = next.AddDays(1);

                var delay = next - now;
                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var casoUso = scope.ServiceProvider.GetRequiredService<ActualizarMoraAcumuladaCasoUso>();
                    await casoUso.Ejecutar(DateTime.Today);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch
                {
                }
            }
        }
    }
}

