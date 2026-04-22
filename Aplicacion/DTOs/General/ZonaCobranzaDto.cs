using System;

namespace FinancieraSoluciones.Application.DTOs.General
{
    public class ZonaCobranzaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
    }
}

