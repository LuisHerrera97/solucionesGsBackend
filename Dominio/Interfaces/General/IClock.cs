using System;

namespace FinancieraSoluciones.Domain.Interfaces.General
{
    public interface IClock
    {
        DateTime Today { get; }
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}
