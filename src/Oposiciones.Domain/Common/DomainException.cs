namespace Oposiciones.Domain.Common;

/// <summary>
/// Error de regla de negocio. La Api lo traduce a un <c>ProblemDetails</c> 4xx en vez de a un 500,
/// de modo que el dominio puede rechazar entradas invalidas sin conocer HTTP.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Se ha pedido un recurso que no existe (examen, tema, test o intento).</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base($"No se ha encontrado {resource} con identificador '{key}'.")
    {
        Resource = resource;
        Key = key;
    }

    public string Resource { get; }

    public object Key { get; }
}

/// <summary>El banco de preguntas no tiene material suficiente para el examen solicitado.</summary>
public sealed class InsufficientQuestionsException : DomainException
{
    public InsufficientQuestionsException(int requested, int available)
        : base($"Se han solicitado {requested} preguntas y el banco solo ofrece {available} que cumplan los filtros.")
    {
        Requested = requested;
        Available = available;
    }

    public int Requested { get; }

    public int Available { get; }
}
