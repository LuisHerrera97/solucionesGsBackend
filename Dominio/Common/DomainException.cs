using System;

namespace FinancieraSoluciones.Domain.Common
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message, int httpCode)
            : base(message)
        {
            HttpCode = httpCode;
        }

        public int HttpCode { get; }
    }

    public sealed class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message, 400) { }
    }

    public sealed class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }

    public sealed class ForbiddenException : DomainException
    {
        public ForbiddenException(string message) : base(message, 403) { }
    }
}
