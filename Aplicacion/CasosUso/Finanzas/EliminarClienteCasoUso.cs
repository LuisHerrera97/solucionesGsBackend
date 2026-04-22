using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class EliminarClienteCasoUso
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarClienteCasoUso(
            IClienteRepositorio clienteRepositorio,
            ICreditoRepositorio creditoRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _clienteRepositorio = clienteRepositorio;
            _creditoRepositorio = creditoRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid id, Guid? usuarioId)
        {
            var cliente = await _clienteRepositorio.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new ArgumentException("No existe el cliente");
            }

            var creditos = await _creditoRepositorio.ContarPorClienteIdAsync(id);
            if (creditos > 0)
            {
                throw new ArgumentException("No se puede eliminar el cliente porque tiene créditos registrados en el sistema.");
            }

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Accion = "EliminarCliente",
                EntidadTipo = "Cliente",
                EntidadId = id,
                Fecha = _clock.UtcNow,
                Detalle = $"Nombre:{cliente.Nombre} {cliente.Apellido}"
            });

            await _clienteRepositorio.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
