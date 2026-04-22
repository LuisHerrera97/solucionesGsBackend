using System;

namespace FinancieraSoluciones.Application.DTOs.General
{
    public class FeriadoDto
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

