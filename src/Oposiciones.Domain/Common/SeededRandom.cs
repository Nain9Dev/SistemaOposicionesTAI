namespace Oposiciones.Domain.Common;

/// <summary>
/// Generador pseudoaleatorio determinista (xorshift128+ sembrado con SplitMix64).
/// <para>
/// No se usa <see cref="Random"/> a proposito: la misma semilla debe producir exactamente
/// el mismo examen en cualquier maquina, runtime y version del framework. Es lo que permite
/// compartir un test por su semilla, reproducir un fallo o fijar un examen en los tests.
/// </para>
/// </summary>
public sealed class SeededRandom : IShuffler
{
    private ulong _state0;
    private ulong _state1;

    public SeededRandom(int seed)
    {
        Seed = seed;

        // SplitMix64 para expandir la semilla de 32 bits a un estado de 128 bits sin ceros.
        ulong z = unchecked((ulong)seed + 0x9E3779B97F4A7C15UL);
        _state0 = Mix(ref z);
        _state1 = Mix(ref z);

        if (_state0 == 0 && _state1 == 0)
        {
            _state1 = 0x9E3779B97F4A7C15UL;
        }
    }

    public int Seed { get; }

    /// <summary>Semilla aleatoria valida para inicializar un examen nuevo.</summary>
    public static int NewSeed() => System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, int.MaxValue);

    /// <summary>Entero en el intervalo semiabierto [0, exclusiveMax).</summary>
    public int Next(int exclusiveMax)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(exclusiveMax, 1);
        return (int)(NextUInt64() % (ulong)exclusiveMax);
    }

    /// <summary>Baraja la lista in-place (Fisher-Yates) de forma determinista.</summary>
    public void Shuffle<T>(IList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    /// <summary>Devuelve una copia barajada sin mutar el origen.</summary>
    public List<T> Shuffled<T>(IEnumerable<T> items)
    {
        var copy = new List<T>(items);
        Shuffle(copy);
        return copy;
    }

    private ulong NextUInt64()
    {
        ulong s1 = _state0;
        ulong s0 = _state1;
        ulong result = unchecked(s0 + s1);

        _state0 = s0;
        s1 ^= s1 << 23;
        _state1 = s1 ^ s0 ^ (s1 >> 18) ^ (s0 >> 5);

        return result;
    }

    private static ulong Mix(ref ulong z)
    {
        unchecked
        {
            z += 0x9E3779B97F4A7C15UL;
            ulong result = z;
            result = (result ^ (result >> 30)) * 0xBF58476D1CE4E5B9UL;
            result = (result ^ (result >> 27)) * 0x94D049BB133111EBUL;
            return result ^ (result >> 31);
        }
    }
}
