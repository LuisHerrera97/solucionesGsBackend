using System;

namespace FinancieraSoluciones.Domain.Entidades.Finanzas
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Negocio { get; set; }
        public string Zona { get; set; }
        public Guid? IdZona { get; set; }
        public string Estatus { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

