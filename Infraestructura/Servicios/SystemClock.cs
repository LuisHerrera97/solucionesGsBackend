using System;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Infraestructura.Servicios
{
    public class SystemClock : IClock
    {
        public DateTime Today => DateTime.Today;
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
