using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class Pagina
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Ruta { get; set; }
        public Guid IdModulo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
        public Modulo Modulo { get; set; }
    }
}
