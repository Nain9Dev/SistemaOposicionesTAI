namespace Oposiciones.Domain.Exceptions;

/// <summary>
/// Error de negocio esperado. El middleware de la API lo traduce a un codigo HTTP concreto,
/// de forma que la capa de dominio no necesita conocer detalles de transporte.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}

/// <summary>Recurso inexistente o no visible para el solicitante.</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string resource, object id) =>
        new($"No se ha encontrado {resource} con identificador '{id}'.");
}

/// <summary>Conflicto con el estado actual del recurso (duplicados, transiciones invalidas).</summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}

/// <summary>El solicitante esta autenticado pero no es propietario del recurso.</summary>
public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}

/// <summary>Entrada sintacticamente valida pero incoherente con las reglas de negocio.</summary>
public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
