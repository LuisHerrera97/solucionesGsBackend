using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class PerfilDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
    }
}