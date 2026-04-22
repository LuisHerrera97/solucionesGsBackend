using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Domain.Entidades.Finanzas
{
    public class Credito
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string Folio { get; set; }
        public Cliente Cliente { get; set; }
        public decimal Monto { get; set; }
        public decimal InteresTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Cuota { get; set; }
        public int TotalFichas { get; set; }
        public decimal Pagado { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool PermitirDomingo { get; set; }
        public bool AplicarFeriados { get; set; }
        public string? Observacion { get; set; }
        public List<Ficha> Fichas { get; set; } = new List<Ficha>();
    }
}
