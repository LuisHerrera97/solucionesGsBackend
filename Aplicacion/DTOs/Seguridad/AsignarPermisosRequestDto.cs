using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class AsignarPermisosRequestDto
    {
        public Guid IdPerfil { get; set; }
        public List<Guid> ModulosPermitidos { get; set; } = new List<Guid>();
        public List<Guid> PaginasPermitidas { get; set; } = new List<Guid>();
        public List<Guid> BotonesPermitidos { get; set; } = new List<Guid>();
    }
}