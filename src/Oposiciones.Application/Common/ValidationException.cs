using Oposiciones.Domain.Common;

namespace Oposiciones.Application.Common;

/// <summary>
/// Entrada invalida detectada antes de llegar al dominio. Agrupa todos los errores de una peticion
/// para que el cliente los reciba de una sola vez y no de uno en uno.
/// </summary>
public sealed class ValidationException : DomainException
{
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("La peticion contiene datos invalidos.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}

/// <summary>Acumulador de errores de validacion con una sintaxis breve para los servicios.</summary>
public sealed class ValidationBuilder
{
    private readonly Dictionary<string, List<string>> _errors = new(StringComparer.OrdinalIgnoreCase);

    public ValidationBuilder Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out List<string>? messages))
        {
            messages = new List<string>();
            _errors[field] = messages;
        }

        messages.Add(message);
        return this;
    }

    /// <summary>Anade el error solo si se cumple la condicion.</summary>
    public ValidationBuilder AddIf(bool condition, string field, string message) =>
        condition ? Add(field, message) : this;

    public bool HasErrors => _errors.Count > 0;

    /// <summary>Lanza <see cref="ValidationException"/> si se acumulo algun error.</summary>
    public void ThrowIfInvalid()
    {
        if (!HasErrors)
        {
            return;
        }

        var snapshot = _errors.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray(),
            StringComparer.OrdinalIgnoreCase);

        throw new ValidationException(snapshot);
    }
}
