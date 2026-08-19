namespace Oposiciones.Domain.Common;

/// <summary>
/// Contrato minimo para reordenar colecciones. Existe para que las entidades no dependan de un
/// generador concreto: en produccion se inyecta <see cref="SeededRandom"/> y en los tests un
/// orden fijo, sin que el dominio note la diferencia.
/// </summary>
public interface IShuffler
{
    List<T> Shuffled<T>(IEnumerable<T> items);
}
