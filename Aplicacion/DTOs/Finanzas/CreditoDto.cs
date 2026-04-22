using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class CreditoDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteApellido { get; set; }
        public string ClienteNegocio { get; set; }
        public string ClienteZona { get; set; }
        public decimal Monto { get; set; }
        public decimal InteresTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Cuota { get; set; }
        public int TotalFichas { get; set; }
        public decimal Pagado { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public bool PermitirDomingo { get; set; }
        public bool AplicarFeriados { get; set; }
        public string? Observacion { get; set; }
        public List<FichaDto> Fichas { get; set; } = new List<FichaDto>();
    }
}
